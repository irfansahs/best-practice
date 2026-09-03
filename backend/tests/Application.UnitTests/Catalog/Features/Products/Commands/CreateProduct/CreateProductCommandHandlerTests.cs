using Application.Catalog.Features.Products.Commands.CreateProduct;
using Application.UnitTests.Helpers;
using Domain.Catalog;
using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Application.UnitTests.Catalog.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesProduct()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var categoryId = Guid.NewGuid();
        var languageId = Guid.NewGuid();
        db.Categories.Add(Category.Create(categoryId, FakeTenantContext.DefaultOrganizationId, FakeTenantContext.DefaultPath).Value);
        db.Languages.Add(Language.Create(languageId, "en", "English", "English").Value);
        await db.SaveChangesAsync();

        var handler = new CreateProductCommandHandler(db, LanguageLookupFactory.Create(db), FakeTenantContext.Default);
        var command = new CreateProductCommand(
            "SKU-100",
            49.99m,
            "USD",
            categoryId,
            languageId,
            "Sample Product",
            "Sample description");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Sku.ShouldBe("SKU-100");
        db.ChangeTracker.Entries<Product>().Count().ShouldBe(1);
        await db.SaveChangesAsync();
        db.Products.ToList().Count.ShouldBe(1);
        var product = db.Products.Single();
        product.Sku.Value.ShouldBe("SKU-100");
        product.Translations.Single().Name.ShouldBe("Sample Product");
    }

    [Fact]
    public async Task Handle_WhenCategoryMissing_ReturnsNotFound()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var handler = new CreateProductCommandHandler(db, LanguageLookupFactory.Create(db), FakeTenantContext.Default);
        var command = new CreateProductCommand(
            "SKU-100",
            49.99m,
            "USD",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sample Product",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.CategoryNotFound);
    }

    [Fact]
    public async Task Handle_WhenLanguageMissing_ReturnsValidationError()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var categoryId = Guid.NewGuid();
        db.Categories.Add(Category.Create(categoryId, FakeTenantContext.DefaultOrganizationId, FakeTenantContext.DefaultPath).Value);
        await db.SaveChangesAsync();

        var handler = new CreateProductCommandHandler(db, LanguageLookupFactory.Create(db), FakeTenantContext.Default);
        var command = new CreateProductCommand(
            "SKU-100",
            49.99m,
            "USD",
            categoryId,
            Guid.NewGuid(),
            "Sample Product",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.TranslationLanguageRequired);
    }

    [Fact]
    public async Task Handle_WhenSkuAlreadyExists_ReturnsConflict()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var categoryId = Guid.NewGuid();
        var languageId = Guid.NewGuid();
        db.Categories.Add(Category.Create(categoryId, FakeTenantContext.DefaultOrganizationId, FakeTenantContext.DefaultPath).Value);
        db.Languages.Add(Language.Create(languageId, "en", "English", "English").Value);
        var existing = Product.Create(
            Guid.NewGuid(),
            Domain.Catalog.ValueObjects.Sku.Create("SKU-100").Value,
            Domain.Catalog.ValueObjects.Money.Create(10m, "USD").Value,
            categoryId,
            FakeTenantContext.DefaultOrganizationId,
            FakeTenantContext.DefaultPath).Value;
        existing.SetTranslation(languageId, "Existing", null);
        db.Products.Add(existing);
        await db.SaveChangesAsync();

        var handler = new CreateProductCommandHandler(db, LanguageLookupFactory.Create(db), FakeTenantContext.Default);
        var command = new CreateProductCommand(
            "SKU-100",
            49.99m,
            "USD",
            categoryId,
            languageId,
            "Another Product",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CatalogErrors.SkuAlreadyExists);
    }
}
