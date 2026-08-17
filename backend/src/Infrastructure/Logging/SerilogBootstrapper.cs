using Infrastructure.Configuration;
using Infrastructure.Logging.Enrichers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Infrastructure.Logging;

public static class SerilogBootstrapper
{
    public static IHostBuilder ConfigureSerilog(IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            var logOptions = context.Configuration.GetSection(LogOptions.SectionName).Get<LogOptions>()
                ?? throw new InvalidOperationException("Log options are not configured.");
            var databaseOptions = context.Configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
                ?? throw new InvalidOperationException("Database options are not configured.");

            if (!Enum.TryParse<LogEventLevel>(logOptions.MinimumLevel, ignoreCase: true, out var minimumLevel))
                minimumLevel = LogEventLevel.Information;

            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .MinimumLevel.Is(minimumLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.With<CorrelationIdEnricher>()
                .Enrich.With(services.GetRequiredService<UserIdEnricher>())
                .Enrich.With(services.GetRequiredService<CultureEnricher>())
                .WriteTo.Console()
                .WriteTo.Async(writeTo =>
                {
                    writeTo.MSSqlServer(
                        connectionString: databaseOptions.ConnectionString,
                        sinkOptions: new MSSqlServerSinkOptions
                        {
                            TableName = "Logs",
                            SchemaName = Persistence.Schemas.Log,
                            AutoCreateSqlTable = true,
                            AutoCreateSqlDatabase = true,
                            BatchPostingLimit = logOptions.BatchSize,
                            BatchPeriod = TimeSpan.FromSeconds(logOptions.BatchPeriodSeconds)
                        },
                        columnOptions: SqlServerLogSchema.CreateColumnOptions());
                });
        });

        return hostBuilder;
    }
}
