using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Localization.Features.Translations.Queries.GetLanguages;
using Api.Extensions;

namespace Api.Endpoints.Localization;

public sealed class LanguageEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/localization/languages").WithTags("Localization");

        group.MapGet("/", async (IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(new GetLanguagesQuery(), ctx, ct))
            .AllowAnonymous()
            .WithName("GetLanguages")
            .Produces<ApiResponse<IReadOnlyList<LanguageDto>>>()
            .WithAnonymousAuthProblems();
    }
}
