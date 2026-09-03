using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Members.Commands.ChangeMemberStatus;

public sealed record ChangeMemberStatusCommand(Guid MembershipId, string Status) : ICommand, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Members.Manage;
}
