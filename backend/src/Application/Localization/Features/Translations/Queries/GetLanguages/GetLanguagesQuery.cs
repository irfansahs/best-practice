using Application.Abstractions.Messaging;

namespace Application.Localization.Features.Translations.Queries.GetLanguages;

public sealed record GetLanguagesQuery : IQuery<IReadOnlyList<LanguageDto>>;
