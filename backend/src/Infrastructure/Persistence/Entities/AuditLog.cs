namespace Infrastructure.Persistence.Entities;

public sealed class AuditLog
{
    public Guid Id { get; set; }

    public string EntityType { get; set; } = null!;

    public string EntityId { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? UserId { get; set; }

    public DateTimeOffset Timestamp { get; set; }
}
