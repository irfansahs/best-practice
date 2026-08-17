using Application.Caching;

namespace Infrastructure.Caching;

public sealed class CacheKeyFactory
{
    public string Product(Guid id, string? culture = null) => CacheKeys.Product(id, culture);

    public string ProductsPaged(string cacheDiscriminator) => CacheKeys.ProductsPaged(cacheDiscriminator);

    public string TranslationResources(string culture) => CacheKeys.TranslationResources(culture);

    public string HealthProbe => CacheKeys.HealthProbe;
}
