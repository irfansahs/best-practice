using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Identity.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage(_ => translator["Identity.Email.Required"]);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).WithMessage(_ => translator["Identity.PasswordHash.Required"]);
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(_ => translator["Identity.FullName.FirstNameRequired"]);
        RuleFor(x => x.LastName).NotEmpty().WithMessage(_ => translator["Identity.FullName.LastNameRequired"]);
    }
}
