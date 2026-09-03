using Api.Endpoints;
using Api.Handlers;
using Application.Dispatching;
using Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;

namespace Api.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddApiOptions(configuration);
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            // Trust reverse proxies in Docker/compose (nginx, ALB). Restrict KnownIPNetworks in hardened prod if needed.
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });
        services.AddEndpoints();
        services.AddCqrs(typeof(CqrsRegistration).Assembly);
        services.AddInfrastructure(configuration);
        services.AddOpenApiServices();
        services.AddRateLimiting();
        services.AddCorsPolicy();
        services.AddResponseCompressionServices();

        return services;
    }
}
