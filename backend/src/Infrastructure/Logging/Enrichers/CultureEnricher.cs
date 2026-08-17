using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Logging.Enrichers;

public sealed class CultureEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var culture = httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.Name ?? "en";
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Culture", culture));
    }
}
