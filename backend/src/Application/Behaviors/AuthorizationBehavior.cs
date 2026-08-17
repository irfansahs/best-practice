using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using SharedKernel.Results;
using Error = SharedKernel.Results.Error;

namespace Application.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse>(ICurrentUser currentUser) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly Error Forbidden = Error.Forbidden("Identity.Authorization.Forbidden", string.Empty);

    public Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IAuthorizedRequest authorized && !currentUser.HasPermission(authorized.Permission))
            return Task.FromResult<Result<TResponse>>(Forbidden);
        return next();
    }
}
