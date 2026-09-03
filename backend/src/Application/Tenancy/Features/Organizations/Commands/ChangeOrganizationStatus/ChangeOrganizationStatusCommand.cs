using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Tenancy.Features.Organizations.Commands.ChangeOrganizationStatus;

public sealed record ChangeOrganizationStatusCommand(Guid Id, string Status) : ICommand, IAuthorizedRequest
{
    public string Permission => Application.Security.Permissions.Tenancy.Organizations.Update;
}
