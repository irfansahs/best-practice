using Application.Abstractions.Caching;
using Application.Abstractions.Localization;
using Application.Caching;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Localization;

public sealed class CachedTranslationProvider(ITranslationProvider inner, ICacheService cache, IOptions<CacheOptions> cacheOptions) : ITranslationProvider
{
    public async Task<string?> GetAsync(string culture, string @namespace, string key, CancellationToken cancellationToken = default) =>
        await cache.GetOrCreateAsync(
            $"{CacheKeys.TranslationResources(culture)}:{@namespace}:{key}",
            async ct => await inner.GetAsync(culture, @namespace, key, ct),
            new CacheEntryOptions { Expiration = cacheOptions.Value.TranslationExpiration },
            [CacheTags.I18n],
            cancellationToken);

    public async Task<IReadOnlyDictionary<string, string>> GetResourcesAsync(string culture, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = @namespace is null
            ? CacheKeys.TranslationResources(culture)
            : $"{CacheKeys.TranslationResources(culture)}:{@namespace}";

        return await cache.GetOrCreateAsync(
            cacheKey,
            async ct => await inner.GetResourcesAsync(culture, @namespace, ct),
            new CacheEntryOptions { Expiration = cacheOptions.Value.TranslationExpiration },
            [CacheTags.I18n],
            cancellationToken);
    }
}
