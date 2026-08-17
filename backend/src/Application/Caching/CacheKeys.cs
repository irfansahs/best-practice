namespace Application.Caching;

public static class CacheKeys
{
    public static string Product(Guid id, string? culture = null) =>
        culture is null ? $"catalog:product:v2:{id}" : $"catalog:product:v2:{id}:{culture}";

    public static string ProductsPaged(string cacheDiscriminator) => $"catalog:products:paged:{cacheDiscriminator}";

    public static string TranslationResources(string culture) => $"localization:resources:{culture}";

    public static string HealthProbe => "health:cache-probe";
}
