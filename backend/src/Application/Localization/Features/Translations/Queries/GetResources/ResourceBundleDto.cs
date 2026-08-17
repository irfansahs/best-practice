namespace Application.Localization.Features.Translations.Queries.GetResources;

public sealed record ResourceBundleDto(string Culture, IReadOnlyDictionary<string, string> Resources);
