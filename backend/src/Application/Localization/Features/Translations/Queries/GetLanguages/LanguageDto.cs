namespace Application.Localization.Features.Translations.Queries.GetLanguages;

public sealed record LanguageDto(Guid Id, string Code, string Name, string NativeName, bool IsDefault, bool IsActive, int SortOrder);
