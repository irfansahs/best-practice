using Application.Catalog.Features.Categories.Queries.GetCategories;
using Application.Catalog.Features.Categories.Queries.GetCategoryById;
using Domain.Catalog;

namespace Application.Catalog.Features.Categories;

public static class CategoryMapper
{
    public static CategoryListItemDto ToListItemDto(Category category, CategoryTranslation? translation) => new(
        category.Id,
        translation?.Name ?? string.Empty,
        category.IsActive,
        category.ParentCategoryId);

    public static CategoryDetailDto ToDetailDto(Category category, CategoryTranslation? translation) => new(
        category.Id,
        category.ParentCategoryId,
        category.IsActive,
        translation?.LanguageId ?? Guid.Empty,
        translation?.Name ?? string.Empty,
        translation?.Description,
        translation?.Slug.Value ?? string.Empty);
}
