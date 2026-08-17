using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Infrastructure.Localization;

public sealed class DbRequestCultureProvider(IServiceScopeFactory scopeFactory) : RequestCultureProvider
{
    public const string CultureHeaderName = "X-Culture";

    public override async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var queryCulture = httpContext.Request.Query["culture"].ToString();
        if (!string.IsNullOrWhiteSpace(queryCulture))
            return new ProviderCultureResult(queryCulture, queryCulture);

        if (httpContext.Request.Headers.TryGetValue(CultureHeaderName, out StringValues headerCulture) && !StringValues.IsNullOrEmpty(headerCulture))
            return new ProviderCultureResult(headerCulture.ToString(), headerCulture.ToString());

        var jwtCulture = httpContext.User.FindFirst("culture")?.Value;
        if (!string.IsNullOrWhiteSpace(jwtCulture))
            return new ProviderCultureResult(jwtCulture, jwtCulture);

        var acceptLanguage = httpContext.Request.Headers.AcceptLanguage.ToString();
        if (!string.IsNullOrWhiteSpace(acceptLanguage))
        {
            var first = acceptLanguage.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(first))
                return new ProviderCultureResult(first, first);
        }

        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var defaultLanguage = await context.Languages.AsNoTracking()
            .Where(l => l.IsDefault && l.IsActive)
            .Select(l => l.Code)
            .FirstOrDefaultAsync(httpContext.RequestAborted);

        return defaultLanguage is null ? null : new ProviderCultureResult(defaultLanguage, defaultLanguage);
    }
}
