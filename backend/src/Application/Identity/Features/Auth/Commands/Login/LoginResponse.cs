namespace Application.Identity.Features.Auth.Commands.Login;

public sealed record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
