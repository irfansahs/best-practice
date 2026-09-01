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
            var cached = await cache.GetAsync<TResponse>(cacheKey, cancellationToken);
            if (cached is not null)
                return Result<TResponse>.Success(cached);

            var result = await next();
            if (result.IsFailure)
                return result;

            await cache.SetAsync(cacheKey, result.Value, options, cachedQuery.Tags, cancellationToken);
            return result;
        }
        catch (Exception)
        {
            await cache.RemoveAsync(cacheKey, cancellationToken);
            return await next();
        }
    }
}
