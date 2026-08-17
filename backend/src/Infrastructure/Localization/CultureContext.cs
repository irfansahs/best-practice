using Application.Abstractions.Localization;
using Domain.Localization.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Infrastructure.Localization;

public sealed class CultureContext(IHttpContextAccessor httpContextAccessor) : ICultureContext
{
    public CultureCode Current
    {
        get
        {
            var feature = httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
            return CultureCode.From(feature?.RequestCulture.Culture.Name);
        }
    }
}
