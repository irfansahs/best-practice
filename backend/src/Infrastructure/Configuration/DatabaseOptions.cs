using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required]
    public string ConnectionString { get; init; } = null!;

    [Range(5, 600)]
    public int CommandTimeout { get; init; } = 30;

    [Range(0, 10)]
    public int MaxRetryCount { get; init; } = 3;

    public TimeSpan MaxRetryDelay { get; init; } = TimeSpan.FromSeconds(5);

    [Range(100, 60000)]
    public int SlowQueryThresholdMs { get; init; } = 1000;
}
