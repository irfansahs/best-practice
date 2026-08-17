using SharedKernel.Results;

namespace Application.Abstractions.Messaging;

public interface IDispatcher
{
    Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
