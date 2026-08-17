using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Catalog.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery : IQuery<IReadOnlyList<CategoryListItemDto>>, IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Categories.Read;
}
