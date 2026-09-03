using Application.Abstractions.Messaging;
using Application.Security;
using Application.Tenancy.Features.Roles.Queries.GetRoles;

namespace Application.Tenancy.Features.Roles.Commands.CreateRole;

public sealed record RolePermissionInput(Guid PermissionId, int Scope);

public sealed record CreateRoleResponse(Guid Id);

public sealed record CreateRoleCommand(
    string Name,
    string? Description,
    int AllowedClients,
    IReadOnlyCollection<RolePermissionInput> Grants) : ICommand<CreateRoleResponse>, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Roles.Manage;
}
