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

        // 2. UseForwardedHeaders (real client IP / proto behind nginx/ALB — before rate limiting)
        app.UseForwardedHeaders();

        // 3. UseHsts + UseHttpsRedirection (skip redirect in Development so HTTP localhost works with Vite)
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        // 4. SecurityHeadersMiddleware
        app.UseMiddleware<SecurityHeadersMiddleware>();

        // 5. UseSerilogRequestLogging
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

        // 6. CorrelationIdMiddleware
        app.UseMiddleware<CorrelationIdMiddleware>();

        // 7. UseResponseCompression
        app.UseResponseCompression();

        // 8. UseRequestLocalization
        app.UseRequestLocalization();

        // CORS must run before authentication for preflight requests
        app.UseCorsPolicy();

        // 9. UseAuthentication
        app.UseAuthentication();

        // 10. UseAuthorization
        app.UseAuthorization();

        // 11. UseRateLimiter
        app.UseRateLimiter();

        // 12. MapEndpoints
        app.MapEndpoints();
        app.MapOpenApiEndpoints();

        return app;
    }
}
