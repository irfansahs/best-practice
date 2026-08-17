using Application.Abstractions.Caching;
using Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace Infrastructure.Caching;

public sealed class HybridCacheService(HybridCache hybridCache, IOptions<CacheOptions> cacheOptions) : ICacheService
{
    public async ValueTask<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        CacheEntryOptions? options = null,
        string[]? tags = null,
        CancellationToken cancellationToken = default)
    {
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = options?.Expiration ?? cacheOptions.Value.DefaultExpiration,
            LocalCacheExpiration = options?.LocalExpiration ?? cacheOptions.Value.DefaultExpiration
        };

        return await hybridCache.GetOrCreateAsync(
            key,
            async ct => await factory(ct),
            entryOptions,
            tags ?? [],
            cancellationToken);
    }

    public ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        hybridCache.RemoveAsync(key, cancellationToken);

    public ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default) =>
        hybridCache.RemoveByTagAsync(tag, cancellationToken);
}
