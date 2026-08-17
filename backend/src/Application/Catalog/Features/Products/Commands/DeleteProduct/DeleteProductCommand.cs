using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Catalog.Features.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : ICommand, IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Products.Delete;
}
