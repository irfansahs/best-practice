using Application.Abstractions.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Primitives;

namespace Infrastructure.Persistence.Interceptors;

public sealed class DomainEventInterceptor(IDomainEventDispatcher dispatcher) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            var domainEvents = CollectDomainEvents(eventData.Context);
            if (domainEvents.Count > 0)
                await dispatcher.DispatchAsync(domainEvents, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static List<SharedKernel.Events.IDomainEvent> CollectDomainEvents(DbContext context)
    {
        var events = new List<SharedKernel.Events.IDomainEvent>();

        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            events.AddRange(entry.Entity.GetDomainEvents());
            entry.Entity.ClearDomainEvents();
        }

        return events;
    }
}
