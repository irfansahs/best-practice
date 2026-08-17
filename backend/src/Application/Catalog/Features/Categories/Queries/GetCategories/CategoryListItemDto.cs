namespace Application.Catalog.Features.Categories.Queries.GetCategories;

public sealed record CategoryListItemDto(Guid Id, string Name, bool IsActive, Guid? ParentCategoryId);
