namespace Application.Catalog.Features.Categories.Queries.GetCategoryById;

public sealed record CategoryDetailDto(
    Guid Id,
    Guid? ParentCategoryId,
    bool IsActive,
    Guid LanguageId,
    string Name,
    string? Description,
    string Slug);
