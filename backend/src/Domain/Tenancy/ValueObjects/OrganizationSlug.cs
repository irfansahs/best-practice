using System.Text.RegularExpressions;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Tenancy.ValueObjects;

public sealed partial class OrganizationSlug : ValueObject
{
    public const int MaxLength = 100;
    public string Value { get; }

    private OrganizationSlug(string value) => Value = value;

    public static Result<OrganizationSlug> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return TenancyErrors.SlugRequired;
        var normalized = value.Trim().ToLowerInvariant();
        if (normalized.Length > MaxLength) return TenancyErrors.SlugTooLong;
        if (!SlugRegex().IsMatch(normalized)) return TenancyErrors.SlugInvalid;
        return new OrganizationSlug(normalized);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex("^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.CultureInvariant)]
    private static partial Regex SlugRegex();
}
