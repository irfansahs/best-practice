using System.Collections.Concurrent;
using Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Results;

namespace Application.Dispatching;

public sealed class HandlerCache
{
    private readonly ConcurrentDictionary<(Type Request, Type Response), object> _cache = new();

    public Func<IServiceProvider, object, CancellationToken, Task<Result<TResponse>>> GetInvoker<TResponse>(Type requestType) =>
        (Func<IServiceProvider, object, CancellationToken, Task<Result<TResponse>>>)_cache.GetOrAdd((requestType, typeof(TResponse)), _ => CreateInvoker<TResponse>(requestType));

    private static Func<IServiceProvider, object, CancellationToken, Task<Result<TResponse>>> CreateInvoker<TResponse>(Type requestType)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));

        return async (sp, request, ct) =>
        {
            var handler = sp.GetRequiredService(handlerType);
            var handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle))!;

            RequestHandlerDelegate<TResponse> handlerDelegate = () =>
                (Task<Result<TResponse>>)handleMethod.Invoke(handler, [request, ct])!;

            var behaviors = sp.GetServices(behaviorType).Reverse().Cast<object>().ToArray();
            var pipeline = handlerDelegate;
            foreach (var behavior in behaviors)
            {
                var next = pipeline;
                var behaviorHandle = behaviorType.GetMethod(nameof(IPipelineBehavior<IRequest<TResponse>, TResponse>.Handle))!;
                pipeline = () => (Task<Result<TResponse>>)behaviorHandle.Invoke(behavior, [request, next, ct])!;
            }

            return await pipeline();
        };
    }
}
