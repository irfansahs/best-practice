using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public TimeSpan DefaultExpiration { get; init; } = TimeSpan.FromMinutes(5);

    public TimeSpan LongExpiration { get; init; } = TimeSpan.FromHours(1);

    public TimeSpan TranslationExpiration { get; init; } = TimeSpan.FromDays(1);
}
