using Application.Catalog.Features.Products.Queries.GetProductById;
using Application.UnitTests.Helpers;
using Domain.Catalog;
using Domain.Localization;
using Shouldly;

namespace Application.UnitTests.Catalog.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductExists_ReturnsDetailDto()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var languageId = Guid.NewGuid();
        db.Languages.Add(Language.Create(languageId, "en", "English", "English", isDefault: true).Value);

        var categoryId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = Product.Create(
            productId,
            Domain.Catalog.ValueObjects.Sku.Create("SKU-200").Value,
            Domain.Catalog.ValueObjects.Money.Create(25m, "EUR").Value,
            categoryId,
            FakeTenantContext.DefaultOrganizationId,
            FakeTenantContext.DefaultPath).Value;
        product.SetTranslation(languageId, "Localized Name", "Localized description");
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var handler = new GetProductByIdQueryHandler(db, LanguageLookupFactory.Create(db));
        var result = await handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(productId);
        result.Value.Sku.ShouldBe("SKU-200");
        result.Value.Price.ShouldBe(25m);
        result.Value.Currency.ShouldBe("EUR");
        result.Value.LanguageId.ShouldBe(languageId);
        result.Value.Name.ShouldBe("Localized Name");
        result.Value.Description.ShouldBe("Localized description");
    }

    [Fact]
    public async Task Handle_WhenProductMissing_ReturnsNotFound()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var handler = new GetProductByIdQueryHandler(db, LanguageLookupFactory.Create(db));

        var result = await handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.ProductNotFound);
    }

    [Fact]
    public async Task Handle_WhenIdEmpty_ReturnsValidationError()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var handler = new GetProductByIdQueryHandler(db, LanguageLookupFactory.Create(db));

        var result = await handler.Handle(new GetProductByIdQuery(Guid.Empty), CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.ProductIdRequired);
    }
}
