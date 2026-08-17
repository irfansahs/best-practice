using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Catalog.ValueObjects;

public sealed class Money : ValueObject
{
    public const int MaxCurrencyLength = 3;
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string? currency)
    {
        if (amount < 0) return CatalogErrors.MoneyAmountInvalid;
        if (string.IsNullOrWhiteSpace(currency)) return CatalogErrors.MoneyCurrencyRequired;
        var normalizedCurrency = currency.Trim().ToUpperInvariant();
        if (normalizedCurrency.Length != MaxCurrencyLength) return CatalogErrors.MoneyCurrencyInvalid;
        return new Money(decimal.Round(amount, 2, MidpointRounding.AwayFromZero), normalizedCurrency);
    }

    public Result<Money> Add(Money other)
    {
        if (Currency != other.Currency) return CatalogErrors.MoneyCurrencyInvalid;
        return Create(Amount + other.Amount, Currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
