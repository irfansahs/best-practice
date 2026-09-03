using Application.Abstractions.Messaging;
using Domain.Identity;

namespace Application.Identity.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password,
    string? IpAddress = null,
    string ClientType = "web",
    Guid? OrganizationId = null,
    string? DeviceId = null,
    string? DeviceName = null) : ICommand<LoginResponse>;
