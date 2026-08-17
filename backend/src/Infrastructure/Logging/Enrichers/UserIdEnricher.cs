using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Logging.Enrichers;

public sealed class UserIdEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var raw = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? user?.FindFirstValue(ClaimTypes.Name);
        if (raw is not null)
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", raw));
    }
}
