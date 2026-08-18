using Application.Abstractions.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Events;
using SharedKernel.Primitives;

namespace Infrastructure.Persistence.Interceptors;

public sealed class DomainEventInterceptor(IDomainEventDispatcher dispatcher) : SaveChangesInterceptor
{
    private List<IDomainEvent>? _pendingEvents;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            _pendingEvents = CollectDomainEvents(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (result > 0 && _pendingEvents is { Count: > 0 })
            await dispatcher.DispatchAsync(_pendingEvents, cancellationToken);

        _pendingEvents = null;
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static List<IDomainEvent> CollectDomainEvents(DbContext context)
    {
        var events = new List<IDomainEvent>();

        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            events.AddRange(entry.Entity.GetDomainEvents());
            entry.Entity.ClearDomainEvents();
        }

        return events;
    }
}
