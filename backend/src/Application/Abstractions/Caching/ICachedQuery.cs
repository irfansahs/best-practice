namespace Application.Abstractions.Caching;

public interface ICachedQuery
{
    string CacheKey { get; }

    TimeSpan Expiration { get; }

    string[] Tags { get; }
}
