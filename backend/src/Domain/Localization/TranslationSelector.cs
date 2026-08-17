using Domain.Abstractions;

namespace Domain.Localization;

public static class TranslationSelector
{
    public static T? SelectForLanguage<T>(this IEnumerable<T> translations, Guid? languageId)
        where T : class, ITranslationEntry
    {
        if (languageId is { } id && id != Guid.Empty)
        {
            var match = translations.FirstOrDefault(t => t.LanguageId == id);
            if (match is not null) return match;
        }

        return translations.FirstOrDefault();
    }
}
