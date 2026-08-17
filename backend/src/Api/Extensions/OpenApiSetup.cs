using Scalar.AspNetCore;

namespace Api.Extensions;

public static class OpenApiSetup
{
    public static IServiceCollection AddOpenApiServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    public static WebApplication MapOpenApiEndpoints(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return app;

        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Best Practice API");
            options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
        });

        return app;
    }
}
