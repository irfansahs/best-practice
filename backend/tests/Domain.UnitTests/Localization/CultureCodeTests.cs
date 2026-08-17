using Domain.Localization;
using Domain.Localization.ValueObjects;
using Shouldly;

namespace Domain.UnitTests.Localization;

public sealed class CultureCodeTests
{
    [Theory]
    [InlineData("en-US", "en")]
    [InlineData("tr-TR", "tr")]
    [InlineData("EN", "en")]
    public void From_NormalizesCulture(string raw, string expected)
    {
        CultureCode.From(raw).Code.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WhenEmpty_ReturnsDefault(string? raw)
    {
        CultureCode.From(raw).ShouldBe(CultureCode.Default);
    }

    [Fact]
    public void Create_WhenEmpty_ReturnsFailure()
    {
        var result = CultureCode.Create("  ");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(LocalizationErrors.LanguageCodeRequired);
    }

    [Fact]
    public void Create_WhenValid_ReturnsCultureCode()
    {
        var result = CultureCode.Create("de-DE");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Code.ShouldBe("de");
    }

    [Fact]
    public void ToString_ReturnsCode()
    {
        CultureCode.From("fr").ToString().ShouldBe("fr");
    }
}
