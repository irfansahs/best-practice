using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Organizations.Commands.CreateOrganization;

public sealed record CreateOrganizationResponse(Guid Id, string Slug);

public sealed record CreateOrganizationCommand(
    string Name,
    string Slug,
    Guid? ParentId,
    string? ContactEmail,
    string? TimeZoneId,
    string? DefaultCulture) : ICommand<CreateOrganizationResponse>, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Organizations.Create;
}
