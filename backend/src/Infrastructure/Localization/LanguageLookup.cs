using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Domain.Localization.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Localization;

public sealed class LanguageLookup(IAppDbContext db, ICultureContext cultureContext) : ILanguageLookup
{
    public async Task<Guid?> GetActiveLanguageIdAsync(CultureCode culture, CancellationToken cancellationToken = default)
    {
        var languageId = await db.Languages.AsNoTracking()
            .Where(l => l.Code == culture.Code && l.IsActive)
            .Select(l => l.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return languageId == Guid.Empty ? null : languageId;
    }

    public Task<Guid?> GetCurrentLanguageIdAsync(CancellationToken cancellationToken = default) =>
        GetActiveLanguageIdAsync(cultureContext.Current, cancellationToken);

    public Task<bool> ExistsAsync(Guid languageId, CancellationToken cancellationToken = default) =>
        db.Languages.AsNoTracking().AnyAsync(l => l.Id == languageId, cancellationToken);
}
