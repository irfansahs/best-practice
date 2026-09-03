using Domain.Abstractions;
using Domain.Catalog.ValueObjects;
using SharedKernel.Abstractions;
using SharedKernel.Auditing;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Catalog;

public sealed class Category : AggregateRoot, IAggregateRoot, IAuditableEntity, ISoftDeletable, ITenantScoped
{
    public const int MaxTranslationNameLength = 200;
    public const int MaxTranslationDescriptionLength = 2000;

    private readonly List<CategoryTranslation> _translations = [];

    public Guid? ParentCategoryId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string OrganizationPath { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public IReadOnlyCollection<CategoryTranslation> Translations => _translations.AsReadOnly();

    private Category() { }

    private Category(Guid id, Guid? parentCategoryId, Guid organizationId, string organizationPath) : base(id)
    {
        ParentCategoryId = parentCategoryId;
        OrganizationId = organizationId;
        OrganizationPath = organizationPath;
        IsActive = true;
    }

    public static Result<Category> Create(Guid id, Guid organizationId, string organizationPath, Guid? parentCategoryId = null)
    {
        if (parentCategoryId == Guid.Empty) return CatalogErrors.CategoryParentInvalid;
        if (organizationId == Guid.Empty || string.IsNullOrWhiteSpace(organizationPath))
            return CatalogErrors.OrganizationRequired;
        return new Category(id, parentCategoryId, organizationId, organizationPath);
    }

    public void AssignTenant(Guid organizationId, string organizationPath)
    {
        if (OrganizationId != Guid.Empty) return;
        OrganizationId = organizationId;
        OrganizationPath = organizationPath;
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
            _translations.Add(CategoryTranslation.Create(Id, languageId, trimmedName, trimmedDescription, slugResult.Value));
        } else {
            existing.Update(trimmedName, trimmedDescription, slugResult.Value);
        }

        return Result.Success();
    }

    public Result AssignParent(Guid? parentCategoryId)
    {
        if (parentCategoryId == Id) return CatalogErrors.CategoryParentInvalid;
        ParentCategoryId = parentCategoryId;
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive) return Result.Success();
        IsActive = true;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive) return Result.Success();
        IsActive = false;
        return Result.Success();
    }

    public void SoftDelete(DateTimeOffset deletedAt, string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt;
        DeletedBy = deletedBy;
        IsActive = false;
    }
}
