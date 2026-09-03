using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Tenancy.Features.Organizations.Commands.CreateOrganization;

public sealed class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(_ => translator["Validation.Required"]);
        RuleFor(x => x.Slug).NotEmpty().WithMessage(_ => translator["Validation.Required"]);
    }
}
