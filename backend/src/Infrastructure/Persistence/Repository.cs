using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class Repository<TAggregate>(AppDbContext context) : IRepository<TAggregate>
    where TAggregate : class, IAggregateRoot
{
    protected AppDbContext Context => context;

    public virtual async Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Set<TAggregate>().FindAsync([id], cancellationToken);

    public void Add(TAggregate aggregate) => context.Set<TAggregate>().Add(aggregate);

    public void Update(TAggregate aggregate) => context.Set<TAggregate>().Update(aggregate);

    public void Delete(TAggregate aggregate) => context.Set<TAggregate>().Remove(aggregate);
}
