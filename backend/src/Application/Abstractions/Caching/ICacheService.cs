namespace Application.Abstractions.Caching;

public interface ICacheService
{
    ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    ValueTask SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions? options = null,
        string[]? tags = null,
        CancellationToken cancellationToken = default);

    ValueTask<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        CacheEntryOptions? options = null,
        string[]? tags = null,
        CancellationToken cancellationToken = default);

    ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default);

    ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default);
}
