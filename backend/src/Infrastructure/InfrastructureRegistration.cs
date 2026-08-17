using System.Text;
using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Events;
using Application.Abstractions.Localization;
using Application.Abstractions.Security;
using Application.Abstractions.Time;
using Application.Catalog.Abstractions;
using Infrastructure.BackgroundJobs;
using Infrastructure.Caching;
using Infrastructure.Configuration;
using Infrastructure.Events;
using Infrastructure.HealthChecks;
using Infrastructure.Localization;
using Infrastructure.Logging;
using Infrastructure.Logging.Enrichers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.Seed;
using Infrastructure.Security;
using Infrastructure.Time;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scrutor;

namespace Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidation<JwtOptions>(configuration);
        services.AddOptionsWithValidation<DatabaseOptions>(configuration);
        services.AddOptionsWithValidation<CacheOptions>(configuration);
        services.AddOptionsWithValidation<LogOptions>(configuration);

        services.AddHttpContextAccessor();
        services.AddScoped<IClock, SystemClock>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ICultureContext, CultureContext>();
        services.AddScoped<ILanguageLookup, LanguageLookup>();
        services.AddScoped<ITranslator, Translator>();
        services.AddSingleton<CacheKeyFactory>();

        services.AddSingleton<UserIdEnricher>();
        services.AddSingleton<CultureEnricher>();

        services.AddScoped<AuditableInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<DomainEventInterceptor>();
        services.AddScoped<AuditLogInterceptor>();
        services.AddScoped<SlowQueryInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) => ConfigureDbContext(sp, options));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IProductRepository, ProductRepository>();
        services.Decorate<IProductRepository, CachedProductRepository>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddHybridCache();
        services.AddScoped<ICacheService, HybridCacheService>();

        services.AddScoped<DbTranslationProvider>();
        services.AddScoped<ITranslationProvider, DbTranslationProvider>();
        services.Decorate<ITranslationProvider, CachedTranslationProvider>();
        services.AddSingleton<IStringLocalizerFactory, DbStringLocalizerFactory>();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((options, jwtOptions) =>
            {
                var settings = jwtOptions.Value;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
        services.AddAuthorization();

        services.AddLocalization();
        services.AddSingleton<DbRequestCultureProvider>();
        services.AddOptions<RequestLocalizationOptions>()
            .Configure<DbRequestCultureProvider>((options, cultureProvider) =>
            {
                options.RequestCultureProviders.Insert(0, cultureProvider);
            });

        services.AddHealthChecks()
            .AddSqlServer(sp => sp.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString, tags: ["ready"])
            .AddCheck<CacheHealthCheck>("cache", tags: ["ready"]);

        services.AddHostedService<LogRetentionService>();
        services.AddHostedService<CacheWarmupService>();

        services.AddScoped<LanguageSeeder>();
        services.AddScoped<TranslationSeeder>();
        services.AddScoped<PermissionSeeder>();
        services.AddScoped<CatalogSeeder>();
        services.AddScoped<IdentitySeeder>();

        return services;
    }

    public static IHostBuilder AddInfrastructureSerilog(this IHostBuilder hostBuilder) =>
        SerilogBootstrapper.ConfigureSerilog(hostBuilder);

    public static async Task SeedDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var languageSeeder = scope.ServiceProvider.GetRequiredService<LanguageSeeder>();
        var translationSeeder = scope.ServiceProvider.GetRequiredService<TranslationSeeder>();
        var permissionSeeder = scope.ServiceProvider.GetRequiredService<PermissionSeeder>();
        var catalogSeeder = scope.ServiceProvider.GetRequiredService<CatalogSeeder>();
        var identitySeeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();

        await languageSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>(), cancellationToken);
        await permissionSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>(), cancellationToken);
        await translationSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>(), cancellationToken);
        await catalogSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>(), cancellationToken);
        await identitySeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>(), cancellationToken);
    }

    private static void ConfigureDbContext(IServiceProvider sp, DbContextOptionsBuilder options)
    {
        var databaseOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        options.UseSqlServer(databaseOptions.ConnectionString, sql =>
        {
            sql.EnableRetryOnFailure(
                maxRetryCount: databaseOptions.MaxRetryCount,
                maxRetryDelay: databaseOptions.MaxRetryDelay,
                errorNumbersToAdd: null);
            sql.CommandTimeout(databaseOptions.CommandTimeout);
            sql.MigrationsHistoryTable("__MigrationsHistory", "dbo");
        });
        options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

        options.AddInterceptors(
            sp.GetRequiredService<AuditableInterceptor>(),
            sp.GetRequiredService<SoftDeleteInterceptor>(),
            sp.GetRequiredService<DomainEventInterceptor>(),
            sp.GetRequiredService<AuditLogInterceptor>(),
            sp.GetRequiredService<SlowQueryInterceptor>());
    }

    private static IServiceCollection AddOptionsWithValidation<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class
    {
        var sectionName = typeof(TOptions).GetField("SectionName")?.GetValue(null)?.ToString()
            ?? throw new InvalidOperationException($"SectionName is missing on {typeof(TOptions).Name}.");

        services.AddOptions<TOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
