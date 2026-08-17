using Application.Abstractions.Messaging;

namespace Application.Identity.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password, string? IpAddress = null) : ICommand<LoginResponse>;
