using Application.Abstractions.Time;

namespace Application.UnitTests.Helpers;

public sealed class FakeTimeProvider : IClock
{
    public DateTimeOffset UtcNow { get; set; } = new(2026, 1, 15, 12, 0, 0, TimeSpan.Zero);
}
