using SharedKernel.Events;

namespace SharedKernel.Primitives;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity() { }

    protected Entity(Guid id) => Id = id;

    public Guid Id { get; protected set; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
