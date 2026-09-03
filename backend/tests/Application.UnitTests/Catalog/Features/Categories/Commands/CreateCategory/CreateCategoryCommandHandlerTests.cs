using Application.Catalog.Features.Categories.Commands.CreateCategory;
using Application.UnitTests.Helpers;
using Domain.Localization;
using Shouldly;

namespace Application.UnitTests.Catalog.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidLanguage_CreatesCategory()
    {
        await using var db = InMemoryDbContextFactory.Create();
        var languageId = Guid.NewGuid();
        db.Languages.Add(Language.Create(languageId, "en", "English", "English").Value);
        await db.SaveChangesAsync();

        var handler = new CreateCategoryCommandHandler(db, LanguageLookupFactory.Create(db), FakeTenantContext.Default);
        var result = await handler.Handle(
            new CreateCategoryCommand(null, languageId, "Electronics", "Devices"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldNotBe(Guid.Empty);
    }
}
