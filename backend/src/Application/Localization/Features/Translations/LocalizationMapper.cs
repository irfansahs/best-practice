using Application.Localization.Features.Translations.Queries.GetLanguages;
using Domain.Localization;
using Riok.Mapperly.Abstractions;

namespace Application.Localization.Features.Translations;

[Mapper]
public static partial class LocalizationMapper
{
    public static LanguageDto ToDto(Language language) => new(
        language.Id,
        language.Code,
        language.Name,
        language.NativeName,
        language.IsDefault,
        language.IsActive,
        language.SortOrder);
}
