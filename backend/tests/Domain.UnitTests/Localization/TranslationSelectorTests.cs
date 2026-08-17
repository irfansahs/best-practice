using Domain.Abstractions;
using Domain.Localization;
using Shouldly;

namespace Domain.UnitTests.Localization;

public sealed class TranslationSelectorTests
{
    private sealed class TestTranslation(Guid languageId) : ITranslationEntry
    {
        public Guid LanguageId { get; } = languageId;
    }

    [Fact]
    public void SelectForLanguage_WhenMatchExists_ReturnsMatchingTranslation()
    {
        var englishId = Guid.NewGuid();
        var turkishId = Guid.NewGuid();
        var translations = new TestTranslation[]
        {
            new(englishId),
            new(turkishId)
        };

        translations.SelectForLanguage(turkishId)!.LanguageId.ShouldBe(turkishId);
    }

    [Fact]
    public void SelectForLanguage_WhenNoMatch_ReturnsFirstTranslation()
    {
        var englishId = Guid.NewGuid();
        var translations = new TestTranslation[] { new(englishId) };

        translations.SelectForLanguage(Guid.NewGuid())!.LanguageId.ShouldBe(englishId);
    }

    [Fact]
    public void SelectForLanguage_WhenLanguageIdEmpty_ReturnsFirstTranslation()
    {
        var englishId = Guid.NewGuid();
        var translations = new TestTranslation[] { new(englishId) };

        translations.SelectForLanguage(Guid.Empty)!.LanguageId.ShouldBe(englishId);
    }

    [Fact]
    public void SelectForLanguage_WhenLanguageIdNull_ReturnsFirstTranslation()
    {
        var englishId = Guid.NewGuid();
        var translations = new TestTranslation[] { new(englishId) };

        translations.SelectForLanguage(null)!.LanguageId.ShouldBe(englishId);
    }
}
