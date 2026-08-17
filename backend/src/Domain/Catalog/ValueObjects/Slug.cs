using System.Text.RegularExpressions;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Catalog.ValueObjects;

public sealed partial class Slug : ValueObject
{
    public const int MaxLength = 128;
    public string Value { get; }

    private Slug(string value) => Value = value;

    public static Result<Slug> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return CatalogErrors.SlugRequired;
        var normalized = value.Trim().ToLowerInvariant();
        if (normalized.Length > MaxLength) return CatalogErrors.SlugTooLong;
        if (!SlugPattern().IsMatch(normalized)) return CatalogErrors.SlugInvalid;
        return new Slug(normalized);
    }

    public static Result<Slug> FromName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return CatalogErrors.SlugRequired;
        var slug = name.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"[\s_]+", "-");
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", string.Empty);
        slug = Regex.Replace(slug, @"\-{2,}", "-").Trim('-');
        return Create(slug);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex("^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled)]
    private static partial Regex SlugPattern();
}
