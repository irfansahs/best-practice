using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Application.Abstractions.Tenancy;
using Domain.Catalog;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(IAppDbContext db, ILanguageLookup languages, ITenantContext tenantContext) : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsAvailable) return TenancyErrors.TenantContextRequired;
        if (request.ParentCategoryId.HasValue && request.ParentCategoryId != Guid.Empty)
        {
            var parentExists = await db.Categories.AsNoTracking()
                .AnyAsync(c => c.Id == request.ParentCategoryId, cancellationToken);
            if (!parentExists) return CatalogErrors.CategoryNotFound;
        }

        if (!await languages.ExistsAsync(request.LanguageId, cancellationToken))
            return CatalogErrors.TranslationLanguageRequired;

        var categoryResult = Category.Create(Guid.NewGuid(), tenantContext.OrganizationId, tenantContext.OrganizationPath, request.ParentCategoryId);
        if (categoryResult.IsFailure) return categoryResult.Error;

        var category = categoryResult.Value;
        var translationResult = category.SetTranslation(request.LanguageId, request.Name, request.Description);
        if (translationResult.IsFailure) return translationResult.Error;

        db.Categories.Add(category);
        return new CreateCategoryResponse(category.Id);
    }
}
