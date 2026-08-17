using Application.Abstractions.Messaging;

namespace Application.Identity.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;
