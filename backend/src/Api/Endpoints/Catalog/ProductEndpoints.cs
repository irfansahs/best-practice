using Application.Abstractions.Messaging;
using Application.Catalog.Features.Products.Commands.ChangeProductPrice;
using Application.Catalog.Features.Products.Commands.CreateProduct;
using Application.Catalog.Features.Products.Commands.DeleteProduct;
using Application.Catalog.Features.Products.Commands.UpdateProduct;
using Application.Catalog.Features.Products.Queries.GetProductById;
using Application.Catalog.Features.Products.Queries.GetProductsPaged;
using Application.Contracts;
using Api.Extensions;
using ProductPerms = Application.Security.Permissions.Catalog.Products;

namespace Api.Endpoints.Catalog;

public sealed class ProductEndpoints : IEndpoint {
    public void MapEndpoint(IEndpointRouteBuilder app) {
        var g = app.MapGroup("/api/v1/catalog/products").WithTags("Catalog");

        g.MapGet("/", ([AsParameters] PageRequest page, string? search, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetProductsPagedQuery(page.Page, page.PageSize, search), ctx, ct))
            .AsQuery<PagedList<ProductListItemDto>>("GetProductsPaged", ProductPerms.Read);

        g.MapGet("/{id:guid}", (Guid id, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetProductByIdQuery(id), ctx, ct))
            .AsGetById<ProductDetailDto>("GetProductById", ProductPerms.Read);

        g.MapPost("/", (CreateProductCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToCreated(cmd, ctx, ct, r => $"/api/v1/catalog/products/{r.Id}"))
            .AsCreate<CreateProductResponse>("CreateProduct", ProductPerms.Create);

        g.MapPut("/{id:guid}", (Guid id, UpdateProductCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(cmd with { Id = id }, ctx, ct))
            .AsUpdate("UpdateProduct", ProductPerms.Update);

        g.MapPut("/{id:guid}/price", (Guid id, ChangeProductPriceCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(cmd with { Id = id }, ctx, ct))
            .AsUpdate("ChangeProductPrice", ProductPerms.Update);

        g.MapDelete("/{id:guid}", (Guid id, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(new DeleteProductCommand(id), ctx, ct))
            .AsDelete("DeleteProduct", ProductPerms.Delete);
    }
}
