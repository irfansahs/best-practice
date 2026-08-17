using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Infrastructure.Localization;

public sealed class DbStringLocalizerFactory(IServiceScopeFactory scopeFactory) : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource) => Create(resourceSource.Name, resourceSource.FullName ?? resourceSource.Name);

    public IStringLocalizer Create(string baseName, string location) => new DbStringLocalizer(baseName, scopeFactory);
}
