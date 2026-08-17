using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Domain.Localization.ValueObjects;
using SharedKernel.Results;

namespace Application.Localization.Features.Translations.Queries.GetResources;

public sealed class GetResourcesQueryHandler(ITranslationProvider translationProvider) : IRequestHandler<GetResourcesQuery, ResourceBundleDto>
{
    public async Task<Result<ResourceBundleDto>> Handle(GetResourcesQuery request, CancellationToken cancellationToken)
    {
        var cultureResult = CultureCode.Create(request.Culture.Code);
        if (cultureResult.IsFailure) return cultureResult.Error;

        var resources = await translationProvider.GetResourcesAsync(cultureResult.Value.Code, request.Namespace, cancellationToken);
        return new ResourceBundleDto(cultureResult.Value.Code, resources);
    }
}
