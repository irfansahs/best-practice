using Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using SharedKernel.Results;

namespace Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Handling {RequestName}", requestName);
        var result = await next();
        if (result.IsFailure)
            logger.LogWarning("Request {RequestName} failed with {ErrorCode}", requestName, result.Error.Code);
        else
            logger.LogInformation("Handled {RequestName}", requestName);
        return result;
    }
}
