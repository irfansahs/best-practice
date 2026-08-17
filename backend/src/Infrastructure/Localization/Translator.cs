using Application.Abstractions.Localization;
using Microsoft.Extensions.Localization;

namespace Infrastructure.Localization;

public sealed class Translator(IStringLocalizerFactory localizerFactory) : ITranslator
{
    private readonly IStringLocalizer _localizer = localizerFactory.Create(typeof(Translator));

    public string this[string key] => Translate(key);

    public string Translate(string key, params object[] args) => args.Length == 0 ? _localizer[key].Value : _localizer[key, args].Value;
}
