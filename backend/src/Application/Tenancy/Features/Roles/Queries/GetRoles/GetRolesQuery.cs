using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Roles.Queries.GetRoles;

public sealed record RolePermissionDto(Guid PermissionId, string Code, int Scope);

public sealed record RoleListItemDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystemRole,
    Guid? OrganizationId,
    int AllowedClients,
    IReadOnlyCollection<RolePermissionDto> Permissions);

public sealed record GetRolesQuery : IQuery<IReadOnlyList<RoleListItemDto>>, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Roles.Read;
}
