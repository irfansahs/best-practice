using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Catalog.Features.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : ICommand<Unit>, IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Categories.Delete;
}
