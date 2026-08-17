using Api.Extensions;

namespace Api.Endpoints.System;

public sealed class HealthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        if (app is WebApplication webApp)
            webApp.MapHealthCheckEndpoints();
    }
}
