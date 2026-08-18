using Domain.Abstractions;
using Domain.Catalog.Events;
using Domain.Catalog.ValueObjects;
using SharedKernel.Abstractions;
using SharedKernel.Auditing;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Catalog;

public sealed class Product : AggregateRoot, IAggregateRoot, IAuditableEntity, ISoftDeletable
{
    public const int MaxTranslationNameLength = 200;
    public const int MaxTranslationDescriptionLength = 4000;

    private readonly List<ProductTranslation> _translations = [];

    public Sku Sku { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public Guid CategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public IReadOnlyCollection<ProductTranslation> Translations => _translations.AsReadOnly();

    private Product() { }

    private Product(Guid id, Sku sku, Money price, Guid categoryId) : base(id)
    {
        Sku = sku;
        Price = price;
        CategoryId = categoryId;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<Product> Create(Guid id, Sku sku, Money price, Guid categoryId)
    {
        if (categoryId == Guid.Empty) return CatalogErrors.CategoryIdRequired;
        var product = new Product(id, sku, price, categoryId);
        product.RaiseDomainEvent(new ProductCreatedEvent(product.Id, product.Sku.Value, product.Price.Amount, product.Price.Currency));
        return product;
    }

    public Result SetTranslation(Guid languageId, string? name, string? description, Slug? slug = null)
    {
        if (languageId == Guid.Empty) return CatalogErrors.TranslationLanguageRequired;
        if (string.IsNullOrWhiteSpace(name)) return CatalogErrors.TranslationNameRequired;
        var trimmedName = name.Trim();
        if (trimmedName.Length > MaxTranslationNameLength) return CatalogErrors.TranslationNameRequired;
        var trimmedDescription = description?.Trim();
        if (trimmedDescription?.Length > MaxTranslationDescriptionLength) return CatalogErrors.TranslationNameRequired;

        var slugResult = slug ?? Slug.FromName(trimmedName);
        if (slugResult.IsFailure) return slugResult.Error;

        var existing = _translations.FirstOrDefault(t => t.LanguageId == languageId);
        if (existing is null)
        {
            _translations.Add(ProductTranslation.Create(Id, languageId, trimmedName, trimmedDescription, slugResult.Value));
        } else {
            existing.Update(trimmedName, trimmedDescription, slugResult.Value);
        }

        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result ChangePrice(Money newPrice)
    {
        if (Price.Amount == newPrice.Amount && Price.Currency == newPrice.Currency) return CatalogErrors.PriceUnchanged;
        var oldPrice = Price;
        Price = newPrice;
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new ProductPriceChangedEvent(Id, oldPrice.Amount, oldPrice.Currency, newPrice.Amount, newPrice.Currency));
        return Result.Success();
    }

    public Result AssignCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty) return CatalogErrors.CategoryIdRequired;
        CategoryId = categoryId;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive) return Result.Success();
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive) return Result.Success();
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public void SoftDelete(DateTimeOffset deletedAt, string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt;
        DeletedBy = deletedBy;
        IsActive = false;
        UpdatedAt = deletedAt;
    }
}
