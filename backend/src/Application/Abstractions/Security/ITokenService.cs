using Domain.Identity;
using Domain.Tenancy;

namespace Application.Abstractions.Security;

public sealed record AccessTokenContext(
    User User,
    Organization Organization,
    PermissionSet Permissions,
    ClientType ClientType,
    bool IsImpersonating = false);

public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(AccessTokenContext context);

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);

    DateTimeOffset GetRefreshTokenExpiresAt(DateTimeOffset issuedAt);

    DateTimeOffset GetImpersonationAccessTokenExpiresAt(DateTimeOffset issuedAt);

    bool ValidateRefreshToken(string refreshToken, string tokenHash);
}
