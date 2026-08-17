using Application.Abstractions.Caching;
using Application.Abstractions.Messaging;
using Application.Caching;
using Application.Security;

namespace Application.Catalog.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductDetailDto>, IAuthorizedRequest, ICachedQuery, ICultureSensitiveCache
{
    public string Permission => Permissions.Catalog.Products.Read;

    public string CacheKey => CacheKeys.Product(Id);
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
    public string[] Tags => [CacheTags.Products];
}
