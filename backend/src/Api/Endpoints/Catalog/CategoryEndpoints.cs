using Application.Abstractions.Messaging;
using Application.Catalog.Features.Categories.Commands.CreateCategory;
using Application.Catalog.Features.Categories.Commands.DeleteCategory;
using Application.Catalog.Features.Categories.Commands.UpdateCategory;
using Application.Catalog.Features.Categories.Queries.GetCategories;
using Application.Catalog.Features.Categories.Queries.GetCategoryById;
using Application.Contracts;
using Application.Security;
using Api.Extensions;

namespace Api.Endpoints.Catalog;

public sealed class CategoryEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/catalog/categories").WithTags("Catalog");

        group.MapGet("/", async (IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(new GetCategoriesQuery(), ctx, ct))
            .WithName("GetCategories")
            .Produces<ApiResponse<IReadOnlyList<CategoryListItemDto>>>()
            .WithDefaultProblems()
            .RequirePermission(Permissions.Catalog.Categories.Read);

        group.MapGet("/{id:guid}", async (Guid id, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(new GetCategoryByIdQuery(id), ctx, ct))
            .WithName("GetCategoryById")
            .Produces<ApiResponse<CategoryDetailDto>>()
            .WithDefaultProblems()
            .WithNotFoundProblem()
            .RequirePermission(Permissions.Catalog.Categories.Read);

        group.MapPost("/", async (CreateCategoryCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToCreated(cmd, ctx, ct, r => $"/api/v1/catalog/categories/{r.Id}"))
            .WithName("CreateCategory")
            .Produces<ApiResponse<CreateCategoryResponse>>(StatusCodes.Status201Created)
            .WithDefaultProblems()
            .WithValidationProblem()
            .RequirePermission(Permissions.Catalog.Categories.Create);

        group.MapPut("/{id:guid}", async (Guid id, UpdateCategoryCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToNoContent(cmd with { Id = id }, ctx, ct))
            .WithName("UpdateCategory")
            .Produces(StatusCodes.Status204NoContent)
            .WithDefaultProblems()
            .WithValidationProblem()
            .WithNotFoundProblem()
            .RequirePermission(Permissions.Catalog.Categories.Update);

        group.MapDelete("/{id:guid}", async (Guid id, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToNoContent(new DeleteCategoryCommand(id), ctx, ct))
            .WithName("DeleteCategory")
            .Produces(StatusCodes.Status204NoContent)
            .WithDefaultProblems()
            .WithNotFoundProblem()
            .WithConflictProblem()
            .RequirePermission(Permissions.Catalog.Categories.Delete);
    }
}
