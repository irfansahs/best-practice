using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Organizations.Commands.UpdateOrganization;

public sealed record UpdateOrganizationCommand(
    Guid Id,
    string Name,
    string? ContactEmail,
    string? TimeZoneId,
    string? DefaultCulture) : ICommand, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Organizations.Update;
}
