using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Catalog.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Sku,
    decimal Price,
    string Currency,
    Guid CategoryId,
    Guid LanguageId,
    string Name,
    string? Description) : ICommand<CreateProductResponse>, IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Products.Create;
}
