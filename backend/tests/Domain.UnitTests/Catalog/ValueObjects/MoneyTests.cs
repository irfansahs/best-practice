using Domain.Catalog;
using Domain.Catalog.ValueObjects;
using Shouldly;

namespace Domain.UnitTests.Catalog.ValueObjects;

public sealed class MoneyTests
{
    [Fact]
    public void Create_WithValidInput_ReturnsRoundedMoney()
    {
        var result = Money.Create(10.555m, "usd");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Amount.ShouldBe(10.56m);
        result.Value.Currency.ShouldBe("USD");
    }

    [Fact]
    public void Create_WithNegativeAmount_ReturnsFailure()
    {
        var result = Money.Create(-1m, "USD");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.MoneyAmountInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingCurrency_ReturnsFailure(string? currency)
    {
        var result = Money.Create(10m, currency);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.MoneyCurrencyRequired);
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDD")]
    public void Create_WithInvalidCurrencyLength_ReturnsFailure(string currency)
    {
        var result = Money.Create(10m, currency);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.MoneyCurrencyInvalid);
    }

    [Fact]
    public void Add_WithSameCurrency_ReturnsSummedMoney()
    {
        var left = Money.Create(10m, "USD").Value;
        var right = Money.Create(5.5m, "USD").Value;

        var result = left.Add(right);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Amount.ShouldBe(15.5m);
        result.Value.Currency.ShouldBe("USD");
    }

    [Fact]
    public void Add_WithDifferentCurrency_ReturnsFailure()
    {
        var left = Money.Create(10m, "USD").Value;
        var right = Money.Create(5m, "EUR").Value;

        var result = left.Add(right);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.MoneyCurrencyInvalid);
    }
}
