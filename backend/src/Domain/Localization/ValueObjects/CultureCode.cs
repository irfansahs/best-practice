using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Localization.ValueObjects;

public sealed class CultureCode : ValueObject
{
    public string Code { get; }

    private CultureCode(string code) => Code = code;

    public static CultureCode Default { get; } = new("en");

    public static CultureCode From(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return Default;

        var code = Normalize(raw);
        return string.IsNullOrEmpty(code) ? Default : new CultureCode(code);
    }

    public static Result<CultureCode> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return LocalizationErrors.LanguageCodeRequired;

        var code = Normalize(raw);
        return string.IsNullOrEmpty(code)
            ? LocalizationErrors.LanguageCodeRequired
            : new CultureCode(code);
    }

    public override string ToString() => Code;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    private static string Normalize(string raw) =>
        raw.Split('-', 2)[0].Trim().ToLowerInvariant();
}
