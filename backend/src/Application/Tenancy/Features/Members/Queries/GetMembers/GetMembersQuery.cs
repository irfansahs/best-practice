using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Members.Queries.GetMembers;

public sealed record MemberListItemDto(
    Guid MembershipId,
    Guid UserId,
    string Email,
    string FullName,
    string Status,
    bool IsPrimary,
    string? Title,
    IReadOnlyCollection<string> Roles);

public sealed record GetMembersQuery(Guid OrganizationId) : IQuery<IReadOnlyList<MemberListItemDto>>, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Members.Read;
}
