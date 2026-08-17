using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Catalog.ValueObjects;

public sealed class Sku : ValueObject
{
    public const int MinLength = 2;
    public const int MaxLength = 64;
    public string Value { get; }

    private Sku(string value) => Value = value;

    public static Result<Sku> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return CatalogErrors.SkuRequired;
        var normalized = value.Trim().ToUpperInvariant();
        if (normalized.Length < MinLength || normalized.Length > MaxLength) return CatalogErrors.SkuInvalid;
        if (!normalized.All(c => char.IsLetterOrDigit(c) || c is '-' or '_')) return CatalogErrors.SkuInvalid;
        return new Sku(normalized);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
