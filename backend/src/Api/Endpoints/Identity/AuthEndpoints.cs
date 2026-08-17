using Api.Endpoints;
using Api.Extensions;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Identity.Features.Auth.Commands.Login;
using Application.Identity.Features.Auth.Commands.Logout;
using Application.Identity.Features.Auth.Commands.RefreshToken;
using Application.Identity.Features.Auth.Commands.Register;
using Application.Identity.Features.Auth.Queries.GetCurrentUser;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints.Identity;

public sealed class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/login", async (LoginCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(cmd with { IpAddress = ctx.Connection.RemoteIpAddress?.ToString() }, ctx, ct))
            .AllowAnonymous()
            .WithName("Login")
            .Produces<ApiResponse<LoginResponse>>()
            .WithValidationProblem()
            .WithAnonymousAuthProblems();

        group.MapPost("/register", async (RegisterCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToCreated(cmd, ctx, ct, _ => "/api/v1/auth/me"))
            .AllowAnonymous()
            .WithName("Register")
            .Produces<ApiResponse<RegisterResponse>>(StatusCodes.Status201Created)
            .WithValidationProblem()
            .WithConflictProblem();

        group.MapPost("/refresh", async (RefreshTokenCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(cmd, ctx, ct))
            .AllowAnonymous()
            .WithName("RefreshToken")
            .Produces<ApiResponse<RefreshTokenResponse>>()
            .WithValidationProblem()
            .WithAnonymousAuthProblems();

        group.MapPost("/logout", async (LogoutCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToNoContent(cmd, ctx, ct))
            .AllowAnonymous()
            .WithName("Logout")
            .Produces(StatusCodes.Status204NoContent)
            .WithAnonymousAuthProblems();

        group.MapGet("/me", async (IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(new GetCurrentUserQuery(), ctx, ct))
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .Produces<ApiResponse<CurrentUserDto>>()
            .WithAnonymousAuthProblems()
            .WithNotFoundProblem();
    }
}
