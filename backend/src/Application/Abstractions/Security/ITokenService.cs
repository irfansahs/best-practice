using Domain.Identity;

namespace Application.Abstractions.Security;

public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(User user);

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);

    DateTimeOffset GetRefreshTokenExpiresAt(DateTimeOffset issuedAt);

    bool ValidateRefreshToken(string refreshToken, string tokenHash);
}
