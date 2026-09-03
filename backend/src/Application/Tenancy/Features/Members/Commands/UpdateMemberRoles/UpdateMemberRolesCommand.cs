using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Members.Commands.UpdateMemberRoles;

public sealed record UpdateMemberRolesCommand(
    Guid MembershipId,
    IReadOnlyCollection<Guid> RoleIds) : ICommand, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Members.Manage;
}
