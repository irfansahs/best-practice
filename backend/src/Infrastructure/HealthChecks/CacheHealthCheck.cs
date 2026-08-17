using Application.Abstractions.Caching;
using Application.Caching;
using Infrastructure.Caching;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.HealthChecks;

public sealed class CacheHealthCheck(ICacheService cache, CacheKeyFactory cacheKeyFactory) : IHealthCheck
{
    private const string ProbeValue = "ok";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var key = cacheKeyFactory.HealthProbe;
        var value = await cache.GetOrCreateAsync(
            key,
            _ => ValueTask.FromResult(ProbeValue),
            cancellationToken: cancellationToken);

        return value == ProbeValue
            ? HealthCheckResult.Healthy("HybridCache is operational.")
            : HealthCheckResult.Unhealthy("HybridCache probe failed.");
    }
}
