using Domain.Abstractions;
using Domain.Catalog.ValueObjects;
using SharedKernel.Guards;
using SharedKernel.Primitives;

namespace Domain.Catalog;

public sealed class ProductTranslation : Entity, ITranslationEntry
{
    public Guid ProductId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Slug Slug { get; private set; } = null!;

    private ProductTranslation() { }

    private ProductTranslation(Guid id, Guid productId, Guid languageId, string name, string? description, Slug slug) : base(id)
    {
        ProductId = productId;
        LanguageId = languageId;
        Name = name;
        Description = description;
        Slug = slug;
    }

    internal static ProductTranslation Create(Guid productId, Guid languageId, string name, string? description, Slug slug) =>
        new(Guid.NewGuid(), productId, languageId, name, description, slug);

    internal void Update(string name, string? description, Slug slug)
    {
        Name = name;
        Description = description;
        Slug = slug;
    }
}
