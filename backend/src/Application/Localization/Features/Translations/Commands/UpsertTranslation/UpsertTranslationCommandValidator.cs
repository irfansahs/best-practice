using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Localization.Features.Translations.Commands.UpsertTranslation;

public sealed class UpsertTranslationCommandValidator : AbstractValidator<UpsertTranslationCommand>
{
    public UpsertTranslationCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.LanguageId).NotEmpty().WithMessage(_ => translator["Localization.Language.NotFound"]);
        RuleFor(x => x.Namespace).NotEmpty().WithMessage(_ => translator["Localization.Translation.NamespaceRequired"]);
        RuleFor(x => x.Key).NotEmpty().WithMessage(_ => translator["Localization.Translation.KeyRequired"]);
        RuleFor(x => x.Value).NotEmpty().WithMessage(_ => translator["Localization.Translation.ValueRequired"]);
    }
}
