using Domain.Identity;
using Domain.Identity.ValueObjects;
using Shouldly;

namespace Domain.UnitTests.Identity.ValueObjects;

public sealed class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ReturnsNormalizedEmail()
    {
        var result = Email.Create("  User@Example.COM  ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe("user@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingValue_ReturnsRequiredError(string? value)
    {
        var result = Email.Create(value);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(IdentityErrors.EmailRequired);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void Create_WithInvalidFormat_ReturnsInvalidError(string value)
    {
        var result = Email.Create(value);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(IdentityErrors.EmailInvalid);
    }

    [Fact]
    public void Create_WithTooLongValue_ReturnsTooLongError()
    {
        var value = new string('a', Email.MaxLength - 10) + "@example.com";

        var result = Email.Create(value);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(IdentityErrors.EmailTooLong);
    }
}
