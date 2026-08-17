using SharedKernel.Events;

namespace Domain.Catalog.Events;

public sealed record ProductCreatedEvent(Guid ProductId, string Sku, decimal Price, string Currency) : DomainEventBase;
