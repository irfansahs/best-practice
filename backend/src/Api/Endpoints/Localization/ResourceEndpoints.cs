using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Localization.Features.Translations.Commands.ImportTranslations;
using Application.Localization.Features.Translations.Commands.UpsertTranslation;
using Application.Localization.Features.Translations.Queries.GetResources;
using Application.Security;
using Api.Extensions;

namespace Api.Endpoints.Localization;

public sealed class ResourceEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/localization").WithTags("Localization");

        group.MapGet("/resources/{culture}", async (string culture, string? @namespace, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(new GetResourcesQuery(culture, @namespace), ctx, ct))
            .AllowAnonymous()
            .WithName("GetResources")
            .Produces<ApiResponse<ResourceBundleDto>>()
            .WithNotFoundProblem();

        group.MapPut("/translations", async (UpsertTranslationCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(cmd, ctx, ct))
            .WithName("UpsertTranslation")
            .Produces<ApiResponse<UpsertTranslationResponse>>()
            .WithDefaultProblems()
            .WithValidationProblem()
            .RequirePermission(Permissions.Localization.Manage);

        group.MapPost("/translations/import", async (ImportTranslationsCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(cmd, ctx, ct))
            .WithName("ImportTranslations")
            .Produces<ApiResponse<ImportTranslationsResponse>>()
            .WithDefaultProblems()
            .WithValidationProblem()
            .RequirePermission(Permissions.Localization.Manage);
    }
}
