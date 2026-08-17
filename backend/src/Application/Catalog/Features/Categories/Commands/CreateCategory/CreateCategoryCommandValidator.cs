using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Catalog.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.LanguageId).NotEmpty().WithMessage(_ => translator["Catalog.Translation.LanguageRequired"]);
        RuleFor(x => x.Name).NotEmpty().WithMessage(_ => translator["Catalog.Translation.NameRequired"]);
    }
}
