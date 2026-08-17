using Domain.Catalog;
using Domain.Catalog.Events;
using Domain.Catalog.ValueObjects;
using Shouldly;

namespace Domain.UnitTests.Catalog;

public sealed class ProductTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();
    private static readonly Guid LanguageId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidInput_ReturnsProductAndRaisesEvent()
    {
        var id = Guid.NewGuid();
        var sku = Sku.Create("SKU-001").Value;
        var price = Money.Create(19.99m, "USD").Value;

        var result = Product.Create(id, sku, price, CategoryId);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(id);
        result.Value.Sku.ShouldBe(sku);
        result.Value.Price.ShouldBe(price);
        result.Value.CategoryId.ShouldBe(CategoryId);
        result.Value.IsActive.ShouldBeTrue();

        var domainEvent = result.Value.GetDomainEvents().ShouldHaveSingleItem();
        domainEvent.ShouldBeOfType<ProductCreatedEvent>();
        var createdEvent = (ProductCreatedEvent)domainEvent;
        createdEvent.ProductId.ShouldBe(id);
        createdEvent.Sku.ShouldBe("SKU-001");
        createdEvent.Price.ShouldBe(19.99m);
        createdEvent.Currency.ShouldBe("USD");
    }

    [Fact]
    public void Create_WithEmptyCategoryId_ReturnsFailure()
    {
        var sku = Sku.Create("SKU-001").Value;
        var price = Money.Create(10m, "USD").Value;

        var result = Product.Create(Guid.NewGuid(), sku, price, Guid.Empty);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.CategoryIdRequired);
    }

    [Fact]
    public void ChangePrice_WithDifferentPrice_UpdatesPriceAndRaisesEvent()
    {
        var product = CreateProduct();
        product.ClearDomainEvents();
        var newPrice = Money.Create(29.99m, "USD").Value;

        var result = product.ChangePrice(newPrice);

        result.IsSuccess.ShouldBeTrue();
        product.Price.ShouldBe(newPrice);

        var domainEvent = product.GetDomainEvents().ShouldHaveSingleItem();
        domainEvent.ShouldBeOfType<ProductPriceChangedEvent>();
        var priceChangedEvent = (ProductPriceChangedEvent)domainEvent;
        priceChangedEvent.OldAmount.ShouldBe(19.99m);
        priceChangedEvent.NewAmount.ShouldBe(29.99m);
    }

    [Fact]
    public void ChangePrice_WithSamePrice_ReturnsFailure()
    {
        var product = CreateProduct();
        var samePrice = Money.Create(19.99m, "USD").Value;

        var result = product.ChangePrice(samePrice);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.PriceUnchanged);
    }

    [Fact]
    public void SetTranslation_WithValidInput_AddsTranslation()
    {
        var product = CreateProduct();

        var result = product.SetTranslation(LanguageId, "Test Product", "A description");

        result.IsSuccess.ShouldBeTrue();
        product.Translations.Count.ShouldBe(1);
        var translation = product.Translations.First();
        translation.Name.ShouldBe("Test Product");
        translation.Description.ShouldBe("A description");
        translation.LanguageId.ShouldBe(LanguageId);
    }

    [Fact]
    public void SetTranslation_WithExistingLanguage_UpdatesTranslation()
    {
        var product = CreateProduct();
        product.SetTranslation(LanguageId, "Original Name", "Original description");

        var result = product.SetTranslation(LanguageId, "Updated Name", "Updated description");

        result.IsSuccess.ShouldBeTrue();
        product.Translations.Count.ShouldBe(1);
        product.Translations.First().Name.ShouldBe("Updated Name");
        product.Translations.First().Description.ShouldBe("Updated description");
    }

    [Fact]
    public void SetTranslation_WithEmptyLanguageId_ReturnsFailure()
    {
        var product = CreateProduct();

        var result = product.SetTranslation(Guid.Empty, "Name", null);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.TranslationLanguageRequired);
    }

    [Fact]
    public void SetTranslation_WithEmptyName_ReturnsFailure()
    {
        var product = CreateProduct();

        var result = product.SetTranslation(LanguageId, "  ", null);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.TranslationNameRequired);
    }

    private static Product CreateProduct()
    {
        var sku = Sku.Create("SKU-001").Value;
        var price = Money.Create(19.99m, "USD").Value;
        return Product.Create(Guid.NewGuid(), sku, price, CategoryId).Value;
    }
}
