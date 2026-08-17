using SharedKernel.Results;

namespace Domain.Localization;

public static class LocalizationErrors
{
    public static Error LanguageNotFound => Error.NotFound("Localization.Language.NotFound", string.Empty);
    public static Error LanguageCodeRequired => Error.Validation("Localization.Language.CodeRequired", string.Empty);
    public static Error LanguageCodeInvalid => Error.Validation("Localization.Language.CodeInvalid", string.Empty);
    public static Error LanguageNameRequired => Error.Validation("Localization.Language.NameRequired", string.Empty);
    public static Error LanguageAlreadyExists => Error.Conflict("Localization.Language.AlreadyExists", string.Empty);
    public static Error LanguageInactive => Error.Conflict("Localization.Language.Inactive", string.Empty);
    public static Error DefaultLanguageRequired => Error.Conflict("Localization.Language.DefaultRequired", string.Empty);

    public static Error TranslationNotFound => Error.NotFound("Localization.Translation.NotFound", string.Empty);
    public static Error TranslationKeyRequired => Error.Validation("Localization.Translation.KeyRequired", string.Empty);
    public static Error TranslationNamespaceRequired => Error.Validation("Localization.Translation.NamespaceRequired", string.Empty);
    public static Error TranslationValueRequired => Error.Validation("Localization.Translation.ValueRequired", string.Empty);
}
