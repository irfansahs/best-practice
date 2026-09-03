using Api.Endpoints;
using Application.Contracts;

namespace Api.Extensions;

public static class EndpointConventions
{
    public static RouteHandlerBuilder AsQuery<T>(this RouteHandlerBuilder b, string name, string permission) =>
        b.WithName(name)
            .Produces<ApiResponse<T>>()
            .WithDefaultProblems()
            .WithValidationProblem()
            .RequirePermission(permission);

    /// <summary>
    /// Command that returns <see cref="ApiResponse{T}"/> with 200 (e.g. upsert/import body responses).
    /// Prefer <see cref="AsCreate{T}"/> when the handler returns 201 Created.
    /// </summary>
    public static RouteHandlerBuilder AsCommand<T>(this RouteHandlerBuilder b, string name, string permission) =>
        b.WithName(name)
            .Produces<ApiResponse<T>>()
            .WithDefaultProblems()
            .WithValidationProblem()
            .RequirePermission(permission);

    public static RouteHandlerBuilder AsGetById<T>(this RouteHandlerBuilder b, string name, string permission) =>
        b.WithName(name)
            .Produces<ApiResponse<T>>()
            .WithDefaultProblems()
            .WithNotFoundProblem()
            .RequirePermission(permission);

    public static RouteHandlerBuilder AsCreate<T>(this RouteHandlerBuilder b, string name, string permission) =>
        b.WithName(name)
            .Produces<ApiResponse<T>>(StatusCodes.Status201Created)
            .WithDefaultProblems()
            .WithValidationProblem()
            .RequirePermission(permission);

    public static RouteHandlerBuilder AsUpdate(this RouteHandlerBuilder b, string name, string permission) =>
        b.WithName(name)
            .Produces(StatusCodes.Status204NoContent)
            .WithDefaultProblems()
            .WithValidationProblem()
            .WithNotFoundProblem()
            .RequirePermission(permission);

    public static RouteHandlerBuilder AsDelete(this RouteHandlerBuilder b, string name, string permission) =>
        b.WithName(name)
            .Produces(StatusCodes.Status204NoContent)
            .WithDefaultProblems()
            .WithNotFoundProblem()
            .RequirePermission(permission);
}
