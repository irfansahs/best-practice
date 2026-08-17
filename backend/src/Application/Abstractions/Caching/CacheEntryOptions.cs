namespace Application.Abstractions.Caching;

public sealed class CacheEntryOptions
{
    public TimeSpan? Expiration { get; init; }
    public TimeSpan? LocalExpiration { get; init; }
}
