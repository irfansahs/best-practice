using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Members.Commands.AddMember;

public sealed record AddMemberResponse(Guid MembershipId);

public sealed record AddMemberCommand(
    Guid OrganizationId,
    Guid UserId,
    IReadOnlyCollection<Guid> RoleIds,
    string? Title,
    bool IsPrimary = false) : ICommand<AddMemberResponse>, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Members.Manage;
}
