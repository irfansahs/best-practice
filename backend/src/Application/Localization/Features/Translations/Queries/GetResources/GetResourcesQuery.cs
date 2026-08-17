using Application.Abstractions.Messaging;
using Domain.Localization.ValueObjects;

namespace Application.Localization.Features.Translations.Queries.GetResources;

public sealed record GetResourcesQuery(CultureCode Culture, string? Namespace = null) : IQuery<ResourceBundleDto>
{
    public GetResourcesQuery(string culture, string? @namespace = null)
        : this(CultureCode.From(culture), @namespace)
    {
    }
}
