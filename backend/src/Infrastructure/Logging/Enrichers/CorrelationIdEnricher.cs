using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Logging.Enrichers;

public sealed class CorrelationIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue("CorrelationId", out var value))
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", value.ToString().Trim('"')));
    }
}
