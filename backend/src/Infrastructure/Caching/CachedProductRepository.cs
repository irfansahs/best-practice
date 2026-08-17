using Application.Abstractions.Caching;
using Application.Caching;
using Application.Catalog.Abstractions;
using Domain.Catalog;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Caching;

public sealed class CachedProductRepository(IProductRepository inner, ICacheService cache, IOptions<CacheOptions> cacheOptions) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await cache.GetOrCreateAsync(
            CacheKeys.Product(id),
            async ct => await inner.GetByIdAsync(id, ct),
            new CacheEntryOptions { Expiration = cacheOptions.Value.LongExpiration },
            [CacheTags.Products],
            cancellationToken);

    public void Add(Product aggregate) => inner.Add(aggregate);

    public void Update(Product aggregate) => inner.Update(aggregate);

    public void Delete(Product aggregate) => inner.Delete(aggregate);
}
