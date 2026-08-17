using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Localization;

public sealed class DbTranslationProvider(IAppDbContext db) : ITranslationProvider
{
    public async Task<string?> GetAsync(string culture, string @namespace, string key, CancellationToken cancellationToken = default) =>
        await (
            from entry in db.TranslationEntries.AsNoTracking()
            join language in db.Languages.AsNoTracking() on entry.LanguageId equals language.Id
            where language.Code == culture && language.IsActive && entry.Namespace == @namespace && entry.Key == key
            select entry.Value).FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<string, string>> GetResourcesAsync(string culture, string? @namespace = null, CancellationToken cancellationToken = default)
    {
        var query =
            from entry in db.TranslationEntries.AsNoTracking()
            join language in db.Languages.AsNoTracking() on entry.LanguageId equals language.Id
            where language.Code == culture && language.IsActive
            select entry;

        if (!string.IsNullOrWhiteSpace(@namespace))
            query = query.Where(e => e.Namespace == @namespace);

        var entries = await query.ToListAsync(cancellationToken);
        return entries.ToDictionary(e => $"{e.Namespace}.{e.Key}", e => e.Value);
    }
}
