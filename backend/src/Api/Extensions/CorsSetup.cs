using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Api.Extensions;

public static class CorsSetup
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors();
        return services;
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        var corsOptions = app.ApplicationServices.GetRequiredService<IOptions<CorsOptions>>().Value;

        if (env.IsDevelopment())
        {
            app.UseCors(policy => policy
                .SetIsOriginAllowed(IsAllowedDevelopmentOrigin)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        }
        else
        {
            app.UseCors(policy => policy
                .WithOrigins(corsOptions.AllowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        }

        return app;
    }

    private static bool IsAllowedDevelopmentOrigin(string origin)
    {
        if (string.IsNullOrEmpty(origin)) return true;

        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;

        return uri.Host is "localhost" or "127.0.0.1"
            || uri.Host.StartsWith("192.168.", StringComparison.Ordinal)
            || uri.Host.StartsWith("10.0.2.", StringComparison.Ordinal);
    }
}
