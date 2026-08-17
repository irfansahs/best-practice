using Application.Abstractions.Caching;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using SharedKernel.Results;

namespace Application.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(ICacheService cache, ICultureContext culture) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICachedQuery cachedQuery)
            return await next();

        var cacheKey = request is ICultureSensitiveCache
            ? $"{cachedQuery.CacheKey}:{culture.Current.Code}"
            : cachedQuery.CacheKey;

        var options = new CacheEntryOptions { Expiration = cachedQuery.Expiration };

        try
        {
            var value = await cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    var result = await next();
                    if (result.IsFailure)
                        throw new UncacheableFailureException(result.Error);

                    return result.Value;
                },
                options,
                cachedQuery.Tags,
                cancellationToken);

            return value;
        }
        catch (UncacheableFailureException ex)
        {
            return ex.Error;
        }
        catch (Exception)
        {
            // Corrupt / incompatible cache payload (e.g. old Result<T> entries) — drop and bypass.
            await cache.RemoveAsync(cacheKey, cancellationToken);
            return await next();
        }
    }

    private sealed class UncacheableFailureException(Error error) : Exception
    {
        public Error Error { get; } = error;
    }
}
