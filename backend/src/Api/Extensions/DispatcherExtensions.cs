using Api.Endpoints;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Extensions;

public static class DispatcherExtensions
{
    public static async Task<Results<Ok<ApiResponse<T>>, ProblemHttpResult>> SendToApiResult<T>(
        this IDispatcher dispatcher,
        IRequest<T> request,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send(request, cancellationToken);
        return result.ToApiResult(httpContext);
    }

    public static async Task<Results<NoContent, ProblemHttpResult>> SendToNoContent(
        this IDispatcher dispatcher,
        IRequest<Unit> request,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send(request, cancellationToken);
        return result.ToNoContentResult(httpContext);
    }

    public static async Task<Results<Created<ApiResponse<T>>, ProblemHttpResult>> SendToCreated<T>(
        this IDispatcher dispatcher,
        IRequest<T> request,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        Func<T, string> locationFactory)
    {
        var result = await dispatcher.Send(request, cancellationToken);
        return result.IsFailure
            ? result.ToProblemDetails(httpContext)
            : result.ToCreatedResult(httpContext, locationFactory(result.Value));
    }
}
