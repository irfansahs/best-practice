using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Organizations.Queries.GetOrganizationById;

public sealed record OrganizationDetailDto(
    Guid Id,
    Guid? ParentId,
    string Name,
    string Slug,
    string Type,
    string Status,
    string Path,
    int Depth,
    string? ContactEmail,
    string TimeZoneId,
    string DefaultCulture);

public sealed record GetOrganizationByIdQuery(Guid Id) : IQuery<OrganizationDetailDto>, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Organizations.Read;
}
