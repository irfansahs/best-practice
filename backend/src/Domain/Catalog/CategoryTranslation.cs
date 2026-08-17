using Domain.Abstractions;
using Domain.Catalog.ValueObjects;
using SharedKernel.Primitives;

namespace Domain.Catalog;

public sealed class CategoryTranslation : Entity, ITranslationEntry
{
    public Guid CategoryId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Slug Slug { get; private set; } = null!;

    private CategoryTranslation() { }

    private CategoryTranslation(Guid id, Guid categoryId, Guid languageId, string name, string? description, Slug slug) : base(id)
    {
        CategoryId = categoryId;
        LanguageId = languageId;
        Name = name;
        Description = description;
        Slug = slug;
    }

    internal static CategoryTranslation Create(Guid categoryId, Guid languageId, string name, string? description, Slug slug) =>
        new(Guid.NewGuid(), categoryId, languageId, name, description, slug);

    internal void Update(string name, string? description, Slug slug)
    {
        Name = name;
        Description = description;
        Slug = slug;
    }
}
