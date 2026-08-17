using Domain.Localization.ValueObjects;

namespace Application.Abstractions.Localization;

public interface ILanguageLookup
{
    Task<Guid?> GetActiveLanguageIdAsync(CultureCode culture, CancellationToken cancellationToken = default);

    Task<Guid?> GetCurrentLanguageIdAsync(CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid languageId, CancellationToken cancellationToken = default);
}
