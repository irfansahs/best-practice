using System.Diagnostics;
using Application.Abstractions.Localization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Handlers;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IServiceScopeFactory scopeFactory) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred while processing {RequestPath}", httpContext.Request.Path);

        using var scope = scopeFactory.CreateScope();
        var translator = scope.ServiceProvider.GetRequiredService<ITranslator>();
        var problem = new ProblemDetails
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
}
