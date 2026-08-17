using Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using SharedKernel.Results;
using System.Diagnostics;

namespace Application.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly TimeSpan Threshold = TimeSpan.FromMilliseconds(500);

    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var result = await next();
        sw.Stop();
        if (sw.Elapsed > Threshold)
            logger.LogWarning("Long running request {RequestName} took {ElapsedMs}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);
        return result;
    }
}
