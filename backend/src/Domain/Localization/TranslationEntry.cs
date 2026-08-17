using SharedKernel.Auditing;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Localization;

public sealed class TranslationEntry : Entity, IAuditableEntity
{
    public const int MaxNamespaceLength = 128;
    public const int MaxKeyLength = 256;
    public const int MaxValueLength = 4000;

    public Guid LanguageId { get; private set; }
    public string Namespace { get; private set; } = null!;
    public string Key { get; private set; } = null!;
    public string Value { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    private TranslationEntry() { }

    private TranslationEntry(Guid id, Guid languageId, string @namespace, string key, string value) : base(id)
    {
        LanguageId = languageId;
        Namespace = @namespace;
        Key = key;
        Value = value;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<TranslationEntry> Create(Guid id, Guid languageId, string? @namespace, string? key, string? value)
    {
        if (languageId == Guid.Empty) return LocalizationErrors.LanguageNotFound;
        if (string.IsNullOrWhiteSpace(@namespace)) return LocalizationErrors.TranslationNamespaceRequired;
        if (string.IsNullOrWhiteSpace(key)) return LocalizationErrors.TranslationKeyRequired;
        if (string.IsNullOrWhiteSpace(value)) return LocalizationErrors.TranslationValueRequired;

        var trimmedNamespace = @namespace.Trim();
        var trimmedKey = key.Trim();
        var trimmedValue = value.Trim();
        if (trimmedNamespace.Length > MaxNamespaceLength || trimmedKey.Length > MaxKeyLength || trimmedValue.Length > MaxValueLength)
            return LocalizationErrors.TranslationValueRequired;

        return new TranslationEntry(id, languageId, trimmedNamespace, trimmedKey, trimmedValue);
    }

    public Result UpdateValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return LocalizationErrors.TranslationValueRequired;
        var trimmedValue = value.Trim();
        if (trimmedValue.Length > MaxValueLength) return LocalizationErrors.TranslationValueRequired;
        Value = trimmedValue;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }
}
