using Application.Abstractions.Messaging;
using Application.Security;

namespace Application.Catalog.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    Guid? ParentCategoryId,
    Guid LanguageId,
    string Name,
    string? Description) : ICommand<CreateCategoryResponse>, IAuthorizedRequest
{
    public string Permission => Permissions.Catalog.Categories.Create;
}
