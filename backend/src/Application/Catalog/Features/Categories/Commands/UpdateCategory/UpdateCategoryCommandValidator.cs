using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Catalog.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(_ => translator["Catalog.Category.IdRequired"]);
        RuleFor(x => x.LanguageId).NotEmpty().WithMessage(_ => translator["Catalog.Translation.LanguageRequired"]);
        RuleFor(x => x.Name).NotEmpty().WithMessage(_ => translator["Catalog.Translation.NameRequired"]);
    }
}
