using SharedKernel.Events;

namespace Domain.Catalog.Events;

public sealed record ProductPriceChangedEvent(
    Guid ProductId,
    decimal OldAmount,
    string OldCurrency,
    decimal NewAmount,
    string NewCurrency) : DomainEventBase;
