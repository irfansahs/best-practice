using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Domain.Localization.ValueObjects;
using Infrastructure.Localization;

namespace Application.UnitTests.Helpers;

public static class LanguageLookupFactory
{
    public static LanguageLookup Create(IAppDbContext db, ICultureContext? cultureContext = null) =>
        new(db, cultureContext ?? new FakeCultureContext(CultureCode.Default));
}
