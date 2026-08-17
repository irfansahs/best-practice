using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Catalog.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    Guid? ParentCategoryId,
    Guid LanguageId,
    string Name,
    string? Description,
    bool IsActive) : ICommand<Unit>, IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Categories.Update;
}
