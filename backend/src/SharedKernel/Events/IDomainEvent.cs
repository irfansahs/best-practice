namespace SharedKernel.Events;

public interface IDomainEvent
{
    Guid Id { get; }

    DateTimeOffset OccurredOn { get; }
}
