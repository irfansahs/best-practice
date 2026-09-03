using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Permissions.Queries.GetPermissionCatalog;

public sealed record PermissionCatalogItemDto(
    Guid Id,
    string Code,
    string? Description,
    string Module,
    int MaxScope,
    bool IsPlatformOnly);

public sealed record GetPermissionCatalogQuery : IQuery<IReadOnlyList<PermissionCatalogItemDto>>, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.PermissionsCatalog.Read;
}
