namespace Domain.Abstractions;

public interface IRepository<TAggregate> where TAggregate : class, IAggregateRoot
{
    Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(TAggregate aggregate);
    void Update(TAggregate aggregate);
    void Delete(TAggregate aggregate);
}
