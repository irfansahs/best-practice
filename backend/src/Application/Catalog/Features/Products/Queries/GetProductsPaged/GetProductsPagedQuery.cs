using Application.Abstractions.Messaging;
using Application.Abstractions.Paged;
using Application.Contracts;
using Application.Security;

namespace Application.Catalog.Features.Products.Queries.GetProductsPaged;

public sealed record GetProductsPagedQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null)
    : PagedQuery(Page, PageSize),
      IQuery<PagedList<ProductListItemDto>>,
      IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Products.Read;
}
