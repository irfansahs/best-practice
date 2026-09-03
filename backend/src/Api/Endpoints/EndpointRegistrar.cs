using System.Reflection;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Api.Endpoints;

public static class EndpointRegistrar
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpointTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IEndpoint).IsAssignableFrom(t));

        foreach (var type in endpointTypes)
            services.AddSingleton(typeof(IEndpoint), type);

        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetServices<IEndpoint>();
        foreach (var endpoint in endpoints)
            endpoint.MapEndpoint(app);

        return app;
    }

    public static RouteHandlerBuilder RequirePermission(
        this RouteHandlerBuilder builder,
        string permission,
        PermissionScope minScope = PermissionScope.Organization) =>
        builder.RequireAuthorization($"Permission:{permission}:{(int)minScope}");
}
