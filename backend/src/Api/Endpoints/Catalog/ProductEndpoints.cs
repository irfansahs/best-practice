using Application.Abstractions.Messaging;
using Application.Catalog.Features.Products.Commands.ChangeProductPrice;
using Application.Catalog.Features.Products.Commands.CreateProduct;
using Application.Catalog.Features.Products.Commands.DeleteProduct;
using Application.Catalog.Features.Products.Commands.UpdateProduct;
using Application.Catalog.Features.Products.Queries.GetProductById;
using Application.Catalog.Features.Products.Queries.GetProductsPaged;
using Application.Contracts;
using Application.Security;
using Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Catalog;

public sealed class ProductEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/catalog/products").WithTags("Catalog");

        group.MapGet("/", async ([AsParameters] PageRequest page, string? search, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(new GetProductsPagedQuery(page, search), ctx, ct))
            .WithName("GetProductsPaged")
            .Produces<ApiResponse<PagedList<ProductListItemDto>>>()
            .WithDefaultProblems()
            .WithValidationProblem()
            .RequirePermission(Permissions.Catalog.Products.Read);

        group.MapGet("/{id:guid}", async (Guid id, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToApiResult(new GetProductByIdQuery(id), ctx, ct))
            .WithName("GetProductById")
            .Produces<ApiResponse<ProductDetailDto>>()
            .WithDefaultProblems()
            .WithNotFoundProblem()
            .RequirePermission(Permissions.Catalog.Products.Read);

        group.MapPost("/", async (CreateProductCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToCreated(cmd, ctx, ct, r => $"/api/v1/catalog/products/{r.Id}"))
            .WithName("CreateProduct")
            .Produces<ApiResponse<CreateProductResponse>>(StatusCodes.Status201Created)
            .WithDefaultProblems()
            .WithValidationProblem()
            .RequirePermission(Permissions.Catalog.Products.Create);

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToNoContent(cmd with { Id = id }, ctx, ct))
            .WithName("UpdateProduct")
            .Produces(StatusCodes.Status204NoContent)
            .WithDefaultProblems()
            .WithValidationProblem()
            .WithNotFoundProblem()
            .RequirePermission(Permissions.Catalog.Products.Update);

        group.MapPut("/{id:guid}/price", async (Guid id, ChangeProductPriceCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToNoContent(cmd with { Id = id }, ctx, ct))
            .WithName("ChangeProductPrice")
            .Produces(StatusCodes.Status204NoContent)
            .WithDefaultProblems()
            .WithValidationProblem()
            .WithNotFoundProblem()
            .RequirePermission(Permissions.Catalog.Products.Update);

        group.MapDelete("/{id:guid}", async (Guid id, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                await d.SendToNoContent(new DeleteProductCommand(id), ctx, ct))
            .WithName("DeleteProduct")
            .Produces(StatusCodes.Status204NoContent)
            .WithDefaultProblems()
            .WithNotFoundProblem()
            .RequirePermission(Permissions.Catalog.Products.Delete);
    }
}
