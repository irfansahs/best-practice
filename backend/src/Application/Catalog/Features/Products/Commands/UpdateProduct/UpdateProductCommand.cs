using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Catalog.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    Guid CategoryId,
    Guid LanguageId,
    string Name,
    string? Description,
    bool IsActive) : ICommand, IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Products.Update;
}
