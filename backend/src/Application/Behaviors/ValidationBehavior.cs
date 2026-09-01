using Application.Abstractions.Messaging;
using FluentValidation;
using SharedKernel.Results;

namespace Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0) return await next();

        var errors = failures
            .GroupBy(f => string.IsNullOrWhiteSpace(f.PropertyName) ? string.Empty : f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).Distinct().ToArray());

        var first = failures[0];
        var code = string.IsNullOrWhiteSpace(first.ErrorCode) ? "Validation.Failed" : first.ErrorCode;
        return Error.Validation(code, first.ErrorMessage, errors);
    }
}
