using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Identity.Features.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage(_ => translator["Identity.Email.Required"]);
        RuleFor(x => x.Password).NotEmpty().WithMessage(_ => translator["Identity.PasswordHash.Required"]);
    }
}
