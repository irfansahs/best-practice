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
        var g = app.MapGroup("/api/v1/localization").WithTags("Localization");

        g.MapGet("/resources/{culture}", (string culture, string? @namespace, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetResourcesQuery(culture, @namespace), ctx, ct))
            .AllowAnonymous()
            .WithName("GetResources")
            .Produces<ApiResponse<ResourceBundleDto>>()
            .WithNotFoundProblem();

        g.MapPut("/translations", (UpsertTranslationCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(cmd, ctx, ct))
            .AsCommand<UpsertTranslationResponse>("UpsertTranslation", Permissions.Localization.Manage);

        g.MapPost("/translations/import", (ImportTranslationsCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(cmd, ctx, ct))
            .AsCommand<ImportTranslationsResponse>("ImportTranslations", Permissions.Localization.Manage);
    }
}
