namespace Application.Abstractions.Localization;

public interface ITranslationProvider
{
    Task<string?> GetAsync(string culture, string @namespace, string key, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, string>> GetResourcesAsync(string culture, string? @namespace = null, CancellationToken cancellationToken = default);
}
