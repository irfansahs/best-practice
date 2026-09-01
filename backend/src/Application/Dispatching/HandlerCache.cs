using System.Collections.Concurrent;
using System.Linq.Expressions;
using Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Results;

namespace Application.Dispatching;

public sealed class HandlerCache
{
    private readonly ConcurrentDictionary<(Type Request, Type Response), object> _cache = new();
    private readonly ConcurrentDictionary<(Type Behavior, Type Request, Type Response), object> _behaviorInvokers = new();
    private readonly ConcurrentDictionary<(Type Handler, Type Request, Type Response), object> _handlerInvokers = new();

    public Func<IServiceProvider, object, CancellationToken, Task<Result<TResponse>>> GetInvoker<TResponse>(Type requestType) =>
        (Func<IServiceProvider, object, CancellationToken, Task<Result<TResponse>>>)_cache.GetOrAdd(
            (requestType, typeof(TResponse)),
            _ => CreateInvoker<TResponse>(requestType));

    private Func<IServiceProvider, object, CancellationToken, Task<Result<TResponse>>> CreateInvoker<TResponse>(Type requestType)
    {
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);

        var compiledHandler = (Func<object, object, CancellationToken, Task<Result<TResponse>>>)_handlerInvokers.GetOrAdd(
            (handlerType, requestType, responseType),
            _ => CompileHandlerInvoke<TResponse>(handlerType, requestType));

        var compiledBehavior = (Func<object, object, RequestHandlerDelegate<TResponse>, CancellationToken, Task<Result<TResponse>>>)_behaviorInvokers.GetOrAdd(
            (behaviorType, requestType, responseType),
            _ => CompileBehaviorInvoke<TResponse>(behaviorType, requestType));

        return async (sp, request, ct) =>
        {
            var handler = sp.GetRequiredService(handlerType);
            var behaviors = sp.GetServices(behaviorType).Reverse().Cast<object>().ToArray();

            RequestHandlerDelegate<TResponse> pipeline = () => compiledHandler(handler, request, ct);

            foreach (var behavior in behaviors)
            {
                var next = pipeline;
                pipeline = () => compiledBehavior(behavior, request, next, ct);
            }

            return await pipeline();
        };
    }

    private static Func<object, object, CancellationToken, Task<Result<TResponse>>> CompileHandlerInvoke<TResponse>(
        Type handlerType,
        Type requestType)
    {
        var handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle))!;

        var handlerParam = Expression.Parameter(typeof(object), "handler");
        var requestParam = Expression.Parameter(typeof(object), "request");
        var ctParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        var call = Expression.Call(
            Expression.Convert(handlerParam, handlerType),
            handleMethod,
            Expression.Convert(requestParam, requestType),
            ctParam);

        return Expression.Lambda<Func<object, object, CancellationToken, Task<Result<TResponse>>>>(
            Expression.Convert(call, typeof(Task<Result<TResponse>>)),
            handlerParam,
            requestParam,
            ctParam).Compile();
    }

    private static Func<object, object, RequestHandlerDelegate<TResponse>, CancellationToken, Task<Result<TResponse>>> CompileBehaviorInvoke<TResponse>(
        Type behaviorType,
        Type requestType)
    {
        var handleMethod = behaviorType.GetMethod(nameof(IPipelineBehavior<IRequest<TResponse>, TResponse>.Handle))!;

        var behaviorParam = Expression.Parameter(typeof(object), "behavior");
        var requestParam = Expression.Parameter(typeof(object), "request");
        var nextParam = Expression.Parameter(typeof(RequestHandlerDelegate<TResponse>), "next");
        var ctParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        var call = Expression.Call(
            Expression.Convert(behaviorParam, behaviorType),
            handleMethod,
            Expression.Convert(requestParam, requestType),
            nextParam,
            ctParam);

        return Expression.Lambda<Func<object, object, RequestHandlerDelegate<TResponse>, CancellationToken, Task<Result<TResponse>>>>(
            Expression.Convert(call, typeof(Task<Result<TResponse>>)),
            behaviorParam,
            requestParam,
            nextParam,
            ctParam).Compile();
    }
}
