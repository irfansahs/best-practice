namespace Application.Identity.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
