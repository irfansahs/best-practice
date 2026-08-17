using Application.Abstractions.Messaging;

namespace Application.Identity.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand;
