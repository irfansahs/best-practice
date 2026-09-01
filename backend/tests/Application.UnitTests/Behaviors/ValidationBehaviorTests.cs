using Application.Abstractions.Messaging;
using Application.Behaviors;
using FluentValidation;
using SharedKernel.Results;
using Shouldly;

namespace Application.UnitTests.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WithMultipleFieldErrors_ReturnsValidationErrorWithDictionary()
    {
        var validators = new IValidator<SampleRequest>[] { new SampleRequestValidator() };
        var behavior = new ValidationBehavior<SampleRequest, string>(validators);
        var request = new SampleRequest(string.Empty, string.Empty);

        var result = await behavior.Handle(request, () => Task.FromResult(Result<string>.Success("ok")), CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Validation);
        result.Error.ValidationErrors.ShouldNotBeNull();
        result.Error.ValidationErrors!.Count.ShouldBe(2);
        result.Error.ValidationErrors.ShouldContainKey(nameof(SampleRequest.Name));
        result.Error.ValidationErrors.ShouldContainKey(nameof(SampleRequest.Email));
    }

    private sealed record SampleRequest(string Name, string Email) : IRequest<string>;

    private sealed class SampleRequestValidator : AbstractValidator<SampleRequest>
    {
        public SampleRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
        }
    }
}
