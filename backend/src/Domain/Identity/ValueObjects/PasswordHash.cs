using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Identity.ValueObjects;

public sealed class PasswordHash : ValueObject
{
    public const int MinLength = 32;
    public const int MaxLength = 512;
    public string Value { get; }

    private PasswordHash(string value) => Value = value;

    public static Result<PasswordHash> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return IdentityErrors.PasswordHashRequired;
        if (value.Length < MinLength || value.Length > MaxLength) return IdentityErrors.PasswordHashInvalid;
        return new PasswordHash(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
