using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Identity.ValueObjects;

public sealed class FullName : ValueObject
{
    public const int MaxPartLength = 100;
    public string FirstName { get; }
    public string LastName { get; }

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<FullName> Create(string? firstName, string? lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) return IdentityErrors.FirstNameRequired;
        if (string.IsNullOrWhiteSpace(lastName)) return IdentityErrors.LastNameRequired;
        var trimmedFirst = firstName.Trim();
        var trimmedLast = lastName.Trim();
        if (trimmedFirst.Length > MaxPartLength || trimmedLast.Length > MaxPartLength) return IdentityErrors.NameTooLong;
        return new FullName(trimmedFirst, trimmedLast);
    }

    public string DisplayName => $"{FirstName} {LastName}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}
