using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>Default key shipped in appsettings.json — must not be used in Production.</summary>
    public const string KnownDevelopmentSecretKey = "dev-secret-key-must-be-at-least-32-chars!";

    [Required]
    public string Issuer { get; init; } = null!;

    [Required]
    public string Audience { get; init; } = null!;

    [Required]
    [MinLength(32)]
    public string SecretKey { get; init; } = null!;

    [Range(1, 1440)]
    public int AccessTokenMinutes { get; init; } = 15;

    [Range(1, 1440)]
    public int ImpersonationAccessTokenMinutes { get; init; } = 30;

    [Range(1, 365)]
    public int RefreshTokenDays { get; init; } = 7;
}
