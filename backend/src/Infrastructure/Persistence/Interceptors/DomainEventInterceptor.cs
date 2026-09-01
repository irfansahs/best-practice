using Application.Abstractions.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Events;
using SharedKernel.Primitives;

namespace Infrastructure.Persistence.Interceptors;

public sealed class DomainEventInterceptor(IDomainEventDispatcher dispatcher) : SaveChangesInterceptor
{
    private List<IDomainEvent>? _pendingEvents;
    private List<Entity>? _entitiesWithEvents;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            SnapshotDomainEvents(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (result > 0 && _pendingEvents is { Count: > 0 })
        {
            await dispatcher.DispatchAsync(_pendingEvents, cancellationToken);
            ClearCollectedDomainEvents();
        }

        _pendingEvents = null;
        _entitiesWithEvents = null;
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void SnapshotDomainEvents(DbContext context)
    {
        _pendingEvents = [];
        _entitiesWithEvents = [];

        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            var events = entry.Entity.GetDomainEvents();
            if (events.Count == 0) continue;

            _entitiesWithEvents.Add(entry.Entity);
            _pendingEvents.AddRange(events);
        }
    }

    private void ClearCollectedDomainEvents()
    {
        if (_entitiesWithEvents is null) return;

        foreach (var entity in _entitiesWithEvents)
            entity.ClearDomainEvents();
    }
}
