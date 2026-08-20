using Application.Abstractions.Messaging;
using Application.Catalog.Features.Categories.Commands.CreateCategory;
using Application.Catalog.Features.Categories.Commands.DeleteCategory;
using Application.Catalog.Features.Categories.Commands.UpdateCategory;
using Application.Catalog.Features.Categories.Queries.GetCategories;
using Application.Catalog.Features.Categories.Queries.GetCategoryById;
using Application.Contracts;
using Api.Extensions;
using CategoryPerms = Application.Security.Permissions.Catalog.Categories;

namespace Api.Endpoints.Catalog;

public sealed class CategoryEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/v1/catalog/categories").WithTags("Catalog");

        g.MapGet("/", (IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetCategoriesQuery(), ctx, ct))
            .AsQuery<IReadOnlyList<CategoryListItemDto>>("GetCategories", CategoryPerms.Read);

        g.MapGet("/{id:guid}", (Guid id, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetCategoryByIdQuery(id), ctx, ct))
            .AsGetById<CategoryDetailDto>("GetCategoryById", CategoryPerms.Read);

        g.MapPost("/", (CreateCategoryCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToCreated(cmd, ctx, ct, r => $"/api/v1/catalog/categories/{r.Id}"))
            .AsCreate<CreateCategoryResponse>("CreateCategory", CategoryPerms.Create);

        g.MapPut("/{id:guid}", (Guid id, UpdateCategoryCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(cmd with { Id = id }, ctx, ct))
            .AsUpdate("UpdateCategory", CategoryPerms.Update);

        g.MapDelete("/{id:guid}", (Guid id, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(new DeleteCategoryCommand(id), ctx, ct))
            .AsDelete("DeleteCategory", CategoryPerms.Delete)
            .WithConflictProblem();
    }
}
