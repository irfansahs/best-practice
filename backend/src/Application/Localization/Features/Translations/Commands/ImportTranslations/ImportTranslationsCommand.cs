using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Localization.Features.Translations.Commands.ImportTranslations;

public sealed record ImportTranslationItem(Guid LanguageId, string Namespace, string Key, string Value);

public sealed record ImportTranslationsCommand(IReadOnlyList<ImportTranslationItem> Items) : ICommand<ImportTranslationsResponse>, IAuthorizedRequest
{
    public string Permission => Permissions.Localization.Manage;
}
