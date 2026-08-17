using Application.Abstractions.Messaging;
using SharedKernel.Results;

namespace Application.Dispatching;

public sealed class Dispatcher(HandlerCache handlerCache, IServiceProvider serviceProvider) : IDispatcher
{
    public Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var invoker = handlerCache.GetInvoker<TResponse>(request.GetType());
        return invoker(serviceProvider, request, cancellationToken);
    }
}
