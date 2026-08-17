namespace SharedKernel.Events;

public abstract record DomainEventBase : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
