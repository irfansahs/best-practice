using System.Diagnostics;
using Application.Abstractions.Localization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;
using ProblemDetailsFactory = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Api.Handlers;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred while processing {RequestPath}", httpContext.Request.Path);

        if (exception is DbUpdateException)
        {
            var conflict = CatalogErrorsFromDbUpdate();
            var conflictProblem = CreateProblem(httpContext, conflict, StatusCodes.Status409Conflict);
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            await httpContext.Response.WriteAsJsonAsync(conflictProblem, cancellationToken);
            return true;
        }

        var translator = httpContext.RequestServices.GetRequiredService<ITranslator>();
        var problem = new ProblemDetailsFactory
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = translator["Api.UnexpectedError"],
            Detail = translator["Api.UnexpectedErrorDetail"],
            Type = "https://httpstatuses.com/500",
            Extensions = { ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier }
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static Error CatalogErrorsFromDbUpdate() =>
        Error.Conflict("Validation.Conflict", "A database constraint was violated.");

    private static ProblemDetailsFactory CreateProblem(HttpContext httpContext, Error error, int status)
    {
        var details = error.ToProblemDetails();
        return new ProblemDetailsFactory
        {
            Status = status,
            Title = details.Title,
            Detail = details.Detail,
            Type = details.Type,
            Extensions =
            {
                ["code"] = error.Code,
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
            }
        };
    }
}
