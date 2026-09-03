namespace Application.Identity.Features.Auth.Queries.GetCurrentUser;

public sealed record OrganizationSummaryDto(
    Guid Id,
    string Name,
    string Slug,
    string Type,
    string Path,
    bool IsPrimary);

public sealed record CurrentUserDto(
    Guid Id,
    string Email,
    string FullName,
    IReadOnlyDictionary<string, int> Permissions,
    OrganizationSummaryDto? ActiveOrganization,
    IReadOnlyCollection<OrganizationSummaryDto> Organizations,
    bool IsImpersonating);
