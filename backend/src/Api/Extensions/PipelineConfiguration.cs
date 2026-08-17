using Api.Endpoints;
using Api.Middlewares;
using Serilog;
using Serilog.Events;

namespace Api.Extensions;

public static class PipelineConfiguration
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // 1. UseExceptionHandler (GlobalExceptionHandler — outermost)
        app.UseExceptionHandler();

        // 2. UseHsts + UseHttpsRedirection (skip redirect in Development so HTTP localhost works with Vite)
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        // 3. SecurityHeadersMiddleware
        app.UseMiddleware<SecurityHeadersMiddleware>();

        // 4. UseSerilogRequestLogging
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (context, _, exception) => exception is not null
                ? LogEventLevel.Error
                : context.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : context.Request.Path.StartsWithSegments("/health")
                        ? LogEventLevel.Verbose
                        : LogEventLevel.Information;
        });

        // 5. CorrelationIdMiddleware
        app.UseMiddleware<CorrelationIdMiddleware>();

        // 6. UseResponseCompression
        app.UseResponseCompression();

        // 7. UseRequestLocalization
        app.UseRequestLocalization();

        // CORS must run before authentication for preflight requests
        app.UseCorsPolicy();

        // 8. UseAuthentication
        app.UseAuthentication();

        // 9. UseAuthorization
        app.UseAuthorization();

        // 10. UseRateLimiter
        app.UseRateLimiter();

        // 11. MapEndpoints
        app.MapEndpoints();
        app.MapOpenApiEndpoints();

        return app;
    }
}
