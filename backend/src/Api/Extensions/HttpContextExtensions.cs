using Microsoft.AspNetCore.Localization;

namespace Api.Extensions;

public static class HttpContextExtensions
{
    public static string GetCulture(this HttpContext httpContext) =>
        httpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.Name ?? "en";
}
