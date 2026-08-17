using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Security;
using Domain.Identity;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;

public sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(User user)
    {
        var options = jwtOptions.Value;
        var permissions = user.Roles.SelectMany(r => r.Permissions).Select(p => p.Code).Distinct(StringComparer.OrdinalIgnoreCase);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(permissions.Select(p => new Claim("permission", p)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(options.AccessTokenMinutes);

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
