using Api.Endpoints;
using Api.Handlers;
using Application.Dispatching;
using Infrastructure;

namespace Api.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddApiOptions(configuration);
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
