using Application.Abstractions.Caching;

namespace Application.UnitTests.Helpers;

public sealed class FakeCacheService : ICacheService
{
    private readonly Dictionary<string, object?> _entries = new(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, object?> Entries => _entries;

    public ValueTask<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        CacheEntryOptions? options = null,
        string[]? tags = null,
        CancellationToken cancellationToken = default)
    {
        if (_entries.TryGetValue(key, out var cached) && cached is T typed)
            return ValueTask.FromResult(typed);

        return CreateAsync(key, factory, cancellationToken);
    }

    public ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _entries.Remove(key);
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

    private async ValueTask<T> CreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        CancellationToken cancellationToken)
    {
        var value = await factory(cancellationToken);
        _entries[key] = value;
        return value;
    }
}
