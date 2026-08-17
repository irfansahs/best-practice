using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration;

public sealed class LogOptions
{
    public const string SectionName = "Log";

    [Range(1, 3650)]
    public int RetentionDays { get; init; } = 30;

    [Range(10, 10000)]
    public int BatchSize { get; init; } = 500;

    [Range(1, 300)]
    public int BatchPeriodSeconds { get; init; } = 5;

    [Required]
    public string MinimumLevel { get; init; } = "Information";
}
