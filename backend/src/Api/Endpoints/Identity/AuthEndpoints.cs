using Api.Extensions;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Identity.Features.Auth.Commands.ChangePassword;
using Application.Identity.Features.Auth.Commands.Login;
using Application.Identity.Features.Auth.Commands.Logout;
using Application.Identity.Features.Auth.Commands.RefreshToken;
using Application.Identity.Features.Auth.Commands.Register;
using Application.Identity.Features.Auth.Commands.SwitchOrganization;
using Application.Identity.Features.Auth.Queries.GetCurrentUser;
using Application.Identity.Features.Auth.Queries.GetMyOrganizations;

namespace Api.Endpoints.Identity;

public sealed class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/login", (LoginCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(cmd with { IpAddress = ctx.Connection.RemoteIpAddress?.ToString() }, ctx, ct))
            .AllowAnonymous()
            .WithName("Login")
            .Produces<ApiResponse<LoginResponse>>()
            .WithValidationProblem()
            .WithAnonymousAuthProblems();

        group.MapPost("/register", (RegisterCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToCreated(cmd, ctx, ct, _ => "/api/v1/auth/me"))
            .AllowAnonymous()
            .WithName("Register")
            .Produces<ApiResponse<RegisterResponse>>(StatusCodes.Status201Created)
            .WithValidationProblem()
            .WithConflictProblem();

        group.MapPost("/refresh", (RefreshTokenCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(cmd, ctx, ct))
            .AllowAnonymous()
            .WithName("RefreshToken")
            .Produces<ApiResponse<RefreshTokenResponse>>()
            .WithValidationProblem()
            .WithAnonymousAuthProblems();

        group.MapPost("/logout", (LogoutCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(cmd, ctx, ct))
            .AllowAnonymous()
            .WithName("Logout")
            .Produces(StatusCodes.Status204NoContent)
            .WithAnonymousAuthProblems();

        group.MapPost("/switch-organization", (SwitchOrganizationCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(cmd, ctx, ct))
            .AllowAnonymous()
            .WithName("SwitchOrganization")
            .Produces<ApiResponse<LoginResponse>>()
            .WithValidationProblem()
            .WithAnonymousAuthProblems();

        group.MapGet("/organizations", (IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetMyOrganizationsQuery(), ctx, ct))
            .RequireAuthorization()
            .WithName("GetMyOrganizations")
            .Produces<ApiResponse<IReadOnlyList<OrganizationSummaryDto>>>()
            .WithAnonymousAuthProblems();

        group.MapGet("/me", (IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetCurrentUserQuery(), ctx, ct))
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .Produces<ApiResponse<CurrentUserDto>>()
            .WithAnonymousAuthProblems()
            .WithNotFoundProblem();

        group.MapPost("/change-password", (ChangePasswordCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(cmd, ctx, ct))
            .RequireAuthorization()
            .WithName("ChangePassword")
            .Produces(StatusCodes.Status204NoContent)
            .WithValidationProblem()
            .WithAnonymousAuthProblems();
    }
}
