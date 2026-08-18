using System.ComponentModel.DataAnnotations;

namespace Application.Configuration;

public sealed class LockoutOptions
{
    public const string SectionName = "Lockout";

    [Range(1, 100)]
    public int MaxFailedAttempts { get; init; } = 5;

    [Range(1, 1440)]
    public int LockoutMinutes { get; init; } = 15;
}
