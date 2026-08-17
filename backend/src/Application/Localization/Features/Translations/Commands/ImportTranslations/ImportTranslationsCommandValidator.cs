using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Localization.Features.Translations.Commands.ImportTranslations;

public sealed class ImportTranslationsCommandValidator : AbstractValidator<ImportTranslationsCommand>
{
    public ImportTranslationsCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage(_ => translator["Localization.Translation.ValueRequired"]);
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.LanguageId).NotEmpty().WithMessage(_ => translator["Localization.Language.NotFound"]);
            item.RuleFor(i => i.Namespace).NotEmpty().WithMessage(_ => translator["Localization.Translation.NamespaceRequired"]);
            item.RuleFor(i => i.Key).NotEmpty().WithMessage(_ => translator["Localization.Translation.KeyRequired"]);
            item.RuleFor(i => i.Value).NotEmpty().WithMessage(_ => translator["Localization.Translation.ValueRequired"]);
        });
    }
}
