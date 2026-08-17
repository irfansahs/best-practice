using Domain.Localization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class TranslationSeeder
{
    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.TranslationEntries.AnyAsync(cancellationToken)) return;

        var entries = new List<TranslationEntry>
        {
            TranslationEntry.Create(Guid.Parse("22222222-2222-2222-2222-222222222201"), LanguageSeeder.EnglishId, "Common", "App.Name", "Best Practice App").Value,
            TranslationEntry.Create(Guid.Parse("22222222-2222-2222-2222-222222222202"), LanguageSeeder.TurkishId, "Common", "App.Name", "En İyi Uygulama").Value,
            TranslationEntry.Create(Guid.Parse("22222222-2222-2222-2222-222222222203"), LanguageSeeder.EnglishId, "Validation", "Required", "This field is required.").Value,
            TranslationEntry.Create(Guid.Parse("22222222-2222-2222-2222-222222222204"), LanguageSeeder.TurkishId, "Validation", "Required", "Bu alan zorunludur.").Value
        };

        context.TranslationEntries.AddRange(entries);
        await context.SaveChangesAsync(cancellationToken);
    }
}
