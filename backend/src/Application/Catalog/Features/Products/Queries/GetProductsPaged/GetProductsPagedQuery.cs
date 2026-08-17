using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Security;

namespace Application.Catalog.Features.Products.Queries.GetProductsPaged;

public sealed record GetProductsPagedQuery(PageRequest Page, string? Search = null) : IQuery<PagedList<ProductListItemDto>>, IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Products.Read;
}
