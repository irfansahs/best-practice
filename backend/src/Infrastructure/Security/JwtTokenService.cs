using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Security;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;

public sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(AccessTokenContext context)
    {
        var options = jwtOptions.Value;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, context.User.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, context.User.Email.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(AuthClaims.SecurityStamp, context.User.SecurityStamp.ToString()),
            new(AuthClaims.OrganizationId, context.Organization.Id.ToString()),
            new(AuthClaims.OrganizationPath, context.Organization.Path),
            new(AuthClaims.OrganizationType, context.Organization.Type.ToString()),
            new(AuthClaims.ClientType, context.ClientType.ToString().ToLowerInvariant())
        };

        if (context.IsImpersonating)
            claims.Add(new Claim(AuthClaims.Impersonating, "1"));

        claims.AddRange(context.Permissions.ToClaimValues().Select(p => new Claim(AuthClaims.Permission, p)));

        var minutes = context.IsImpersonating ? options.ImpersonationAccessTokenMinutes : options.AccessTokenMinutes;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(minutes);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return (TokenHandler.WriteToken(token), expires);
    }

    public DateTimeOffset GetRefreshTokenExpiresAt(DateTimeOffset issuedAt) =>
        issuedAt.AddDays(jwtOptions.Value.RefreshTokenDays);

    public DateTimeOffset GetImpersonationAccessTokenExpiresAt(DateTimeOffset issuedAt) =>
        issuedAt.AddMinutes(jwtOptions.Value.ImpersonationAccessTokenMinutes);

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public bool ValidateRefreshToken(string refreshToken, string tokenHash) =>
        !string.IsNullOrWhiteSpace(refreshToken) && HashRefreshToken(refreshToken) == tokenHash;

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(bytes);
    }
}
