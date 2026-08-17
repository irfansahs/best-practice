using Domain.Abstractions;
using SharedKernel.Auditing;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Localization;

public sealed class Language : AggregateRoot, IAggregateRoot, IAuditableEntity
{
    public const int MaxCodeLength = 10;
    public const int MaxNameLength = 64;
    public const int MaxNativeNameLength = 64;

    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string NativeName { get; private set; } = null!;
    public bool IsDefault { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    private Language() { }

    private Language(Guid id, string code, string name, string nativeName, bool isDefault, int sortOrder) : base(id)
    {
        Code = code;
        Name = name;
        NativeName = nativeName;
        IsDefault = isDefault;
        IsActive = true;
        SortOrder = sortOrder;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<Language> Create(Guid id, string? code, string? name, string? nativeName, bool isDefault = false, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code)) return LocalizationErrors.LanguageCodeRequired;
        if (string.IsNullOrWhiteSpace(name)) return LocalizationErrors.LanguageNameRequired;
        if (string.IsNullOrWhiteSpace(nativeName)) return LocalizationErrors.LanguageNameRequired;

        var normalizedCode = code.Trim().ToLowerInvariant();
        if (normalizedCode.Length > MaxCodeLength || !normalizedCode.All(c => char.IsLetter(c) || c == '-')) return LocalizationErrors.LanguageCodeInvalid;

        var trimmedName = name.Trim();
        var trimmedNativeName = nativeName.Trim();
        if (trimmedName.Length > MaxNameLength || trimmedNativeName.Length > MaxNativeNameLength) return LocalizationErrors.LanguageNameRequired;

        return new Language(id, normalizedCode, trimmedName, trimmedNativeName, isDefault, sortOrder);
    }

    public Result Activate()
    {
        if (IsActive) return Result.Success();
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (IsDefault) return LocalizationErrors.DefaultLanguageRequired;
        if (!IsActive) return Result.Success();
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result MarkAsDefault()
    {
        IsDefault = true;
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result UnmarkAsDefault()
    {
        if (!IsDefault) return Result.Success();
        IsDefault = false;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }
}
