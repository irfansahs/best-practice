using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Tenancy.Features.Members.Commands.AddMember;

public sealed class AddMemberCommandValidator : AbstractValidator<AddMemberCommand>
{
    public AddMemberCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage(_ => translator["Validation.Required"]);
        RuleFor(x => x.RoleIds).NotEmpty().WithMessage(_ => translator["Validation.Required"]);
    }
}
