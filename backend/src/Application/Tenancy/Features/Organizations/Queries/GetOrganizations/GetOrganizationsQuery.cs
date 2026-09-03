using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Organizations.Queries.GetOrganizations;

public sealed record OrganizationListItemDto(
    Guid Id,
    Guid? ParentId,
    string Name,
    string Slug,
    string Type,
    string Status,
    string Path,
    int Depth);

public sealed record GetOrganizationsQuery : IQuery<IReadOnlyList<OrganizationListItemDto>>, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Organizations.Read;
}
