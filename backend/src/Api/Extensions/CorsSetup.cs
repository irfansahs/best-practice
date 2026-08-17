using Microsoft.Extensions.Options;

namespace Api.Extensions;

public static class CorsSetup
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors();
        return services;
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        var corsOptions = app.ApplicationServices.GetRequiredService<IOptions<CorsOptions>>().Value;

        app.UseCors(policy => policy
            .WithOrigins(corsOptions.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());

        return app;
    }
}
