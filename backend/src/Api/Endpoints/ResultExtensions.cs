using System.Diagnostics;
using Api.Extensions;
using Application.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Results;
using ProblemDetailsFactory = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Api.Endpoints;

public static class ResultExtensions
{
    public static ProblemHttpResult ToProblemDetails(this Result result) =>
        TypedResults.Problem(CreateProblemDetails(result.Error, null));

    public static ProblemHttpResult ToProblemDetails<T>(this Result<T> result, HttpContext? httpContext = null) =>
        TypedResults.Problem(CreateProblemDetails(result.Error, httpContext));

    public static Ok<ApiResponse<T>> ToOkResponse<T>(this Result<T> result, HttpContext httpContext) =>
        TypedResults.Ok(ApiResponse<T>.Ok(result.Value, CreateMeta(httpContext)));

    public static Results<Ok<ApiResponse<T>>, ProblemHttpResult> ToApiResult<T>(this Result<T> result, HttpContext httpContext) =>
        result.IsSuccess ? result.ToOkResponse(httpContext) : result.ToProblemDetails(httpContext);

    public static Results<Created<ApiResponse<T>>, ProblemHttpResult> ToCreatedResult<T>(
        this Result<T> result,
        HttpContext httpContext,
        string location) =>
        result.IsSuccess
            ? TypedResults.Created(location, ApiResponse<T>.Ok(result.Value, CreateMeta(httpContext)))
            : result.ToProblemDetails(httpContext);

    public static Results<NoContent, ProblemHttpResult> ToNoContentResult(this Result<Application.Abstractions.Messaging.Unit> result, HttpContext? httpContext = null) =>
        result.IsSuccess ? TypedResults.NoContent() : result.ToProblemDetails(httpContext);

    private static ProblemDetailsFactory CreateProblemDetails(Error error, HttpContext? httpContext)
    {
        var details = error.ToProblemDetails();
        var extensions = new Dictionary<string, object?>
        {
            ["code"] = error.Code,
            ["traceId"] = Activity.Current?.Id ?? httpContext?.TraceIdentifier
        };

        if (error.ValidationErrors is not null)
            extensions["errors"] = error.ValidationErrors;

        return new ProblemDetailsFactory
        {
            Status = details.Status,
            Title = details.Title,
            Detail = details.Detail,
            Type = details.Type,
            Extensions = extensions
        };
    }

    private static ApiMeta CreateMeta(HttpContext httpContext) =>
        new(Activity.Current?.Id ?? httpContext.TraceIdentifier, httpContext.GetCulture());
}
