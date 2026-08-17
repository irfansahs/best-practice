using Domain.Localization;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class LanguageSeeder
{
    public static readonly Guid EnglishId = Guid.Parse("11111111-1111-1111-1111-111111111101");
    public static readonly Guid TurkishId = Guid.Parse("11111111-1111-1111-1111-111111111102");

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Languages.AnyAsync(cancellationToken)) return;

        var english = Language.Create(EnglishId, "en", "English", "English", isDefault: true, sortOrder: 1).Value;
        var turkish = Language.Create(TurkishId, "tr", "Turkish", "Türkçe", sortOrder: 2).Value;
        context.Languages.AddRange(english, turkish);
        await context.SaveChangesAsync(cancellationToken);
    }
}
