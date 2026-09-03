using Application.Abstractions.Messaging;
using Application.Identity.Features.Auth.Commands.Login;

namespace Application.Identity.Features.Auth.Commands.SwitchOrganization;

public sealed record SwitchOrganizationCommand(
    Guid OrganizationId,
    string RefreshToken,
    string ClientType = "web") : ICommand<LoginResponse>;
