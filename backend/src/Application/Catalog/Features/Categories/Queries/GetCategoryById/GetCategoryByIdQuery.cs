using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Catalog.Features.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryDetailDto>, IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Categories.Read;
}
