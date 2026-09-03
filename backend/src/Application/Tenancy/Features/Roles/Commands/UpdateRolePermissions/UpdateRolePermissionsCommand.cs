using Application.Abstractions.Messaging;
using Application.Security;
using Application.Tenancy.Features.Roles.Commands.CreateRole;

namespace Application.Tenancy.Features.Roles.Commands.UpdateRolePermissions;

public sealed record UpdateRolePermissionsCommand(
    Guid RoleId,
    IReadOnlyCollection<RolePermissionInput> Grants) : ICommand, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Roles.Manage;
}
