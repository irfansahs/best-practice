using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Catalog.Features.Products.Commands.ChangeProductPrice;

public sealed record ChangeProductPriceCommand(Guid Id, decimal Price, string Currency) : ICommand, IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Products.Update;
}
