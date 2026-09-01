using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using SharedKernel.Results;

namespace Application.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICommand && request is not ICommand<TResponse>)
            return await next();

        var result = await next();
        if (result.IsSuccess)
            await unitOfWork.SaveChangesAsync(cancellationToken);

        return result;
    }
}
