using Application.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Events;

namespace Infrastructure.Events;

public sealed class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                if (handler is null) continue;
                await InvokeHandlerAsync(handler, domainEvent, cancellationToken);
            }
        }
    }

    private static async Task InvokeHandlerAsync(object handler, IDomainEvent domainEvent, CancellationToken cancellationToken) =>
        await ((dynamic)handler).Handle((dynamic)domainEvent, cancellationToken);
}
