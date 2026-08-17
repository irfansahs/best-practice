using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Localization.Features.Translations.Commands.UpsertTranslation;

public sealed record UpsertTranslationCommand(
    Guid LanguageId,
    string Namespace,
    string Key,
    string Value) : ICommand<UpsertTranslationResponse>, IAuthorizedRequest
{
    public string Permission => Permissions.Localization.Manage;
}
