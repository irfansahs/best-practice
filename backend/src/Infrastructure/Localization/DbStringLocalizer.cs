using Application.Abstractions.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Infrastructure.Localization;

public sealed class DbStringLocalizer(string @namespace, IServiceScopeFactory scopeFactory) : IStringLocalizer
{
    public LocalizedString this[string name]
    {
        get
        {
            using var scope = scopeFactory.CreateScope();
            var translationProvider = scope.ServiceProvider.GetRequiredService<ITranslationProvider>();
            var cultureContext = scope.ServiceProvider.GetRequiredService<ICultureContext>();
            var value = translationProvider.GetAsync(cultureContext.Current.Code, @namespace, name).GetAwaiter().GetResult();
            return new LocalizedString(name, value ?? name, resourceNotFound: value is null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var localized = this[name];
            return new LocalizedString(name, string.Format(localized.Value, arguments), localized.ResourceNotFound);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        using var scope = scopeFactory.CreateScope();
        var translationProvider = scope.ServiceProvider.GetRequiredService<ITranslationProvider>();
        var cultureContext = scope.ServiceProvider.GetRequiredService<ICultureContext>();
        return translationProvider.GetResourcesAsync(cultureContext.Current.Code, @namespace).GetAwaiter().GetResult()
            .Select(pair => new LocalizedString(pair.Key, pair.Value, resourceNotFound: false));
    }

    public IStringLocalizer WithCulture(System.Globalization.CultureInfo culture) => this;
}
