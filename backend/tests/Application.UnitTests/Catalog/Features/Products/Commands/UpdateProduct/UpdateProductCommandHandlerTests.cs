using Application.Catalog.Features.Products.Commands.UpdateProduct;
using Application.UnitTests.Helpers;
using Domain.Catalog;
using Domain.Localization;
using Shouldly;

namespace Application.UnitTests.Catalog.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCategoryMissing_ReturnsNotFound()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var languageId = Guid.NewGuid();
        db.Languages.Add(Language.Create(languageId, "en", "English", "English").Value);

        var product = Product.Create(
            Guid.NewGuid(),
            Domain.Catalog.ValueObjects.Sku.Create("SKU-1").Value,
            Domain.Catalog.ValueObjects.Money.Create(10m, "USD").Value,
            Guid.NewGuid()).Value;
        product.SetTranslation(languageId, "Name", null);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var handler = new UpdateProductCommandHandler(db, LanguageLookupFactory.Create(db));
        var result = await handler.Handle(
            new UpdateProductCommand(product.Id, Guid.NewGuid(), languageId, "Name", null, true),
            CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.CategoryNotFound);
    }
}
