using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Identity.ValueObjects;

public sealed class Email : ValueObject
{
    public const int MaxLength = 256;
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return IdentityErrors.EmailRequired;
        var normalized = value.Trim().ToLowerInvariant();
        if (normalized.Length > MaxLength) return IdentityErrors.EmailTooLong;
        if (!normalized.Contains('@') || normalized.StartsWith('@') || normalized.EndsWith('@')) return IdentityErrors.EmailInvalid;
        return new Email(normalized);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
