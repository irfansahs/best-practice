using Domain.Identity;
using Domain.Identity.ValueObjects;
using Shouldly;

namespace Domain.UnitTests.Identity;

public sealed class RefreshTokenTests
{
    [Fact]
    public void IssueAndRevoke_MarksTokenInactive_AndRecordsReplacement()
    {
        var user = CreateUser();
        var createdAt = DateTimeOffset.UtcNow;
        var token = user.IssueRefreshToken(Guid.NewGuid(), "hash", createdAt.AddDays(7), createdAt, Guid.NewGuid(), Guid.NewGuid(), ClientType.Web);
        var replacementId = Guid.NewGuid();

        token.IsActive(createdAt).ShouldBeTrue();
        token.Revoke(createdAt.AddMinutes(1), replacementId);

        token.IsRevoked.ShouldBeTrue();
        token.ReplacedByTokenId.ShouldBe(replacementId);
        token.IsActive(createdAt.AddMinutes(2)).ShouldBeFalse();
    }

    [Fact]
    public void IsExpired_WhenPastExpiresAt_ReturnsTrue()
    {
        var user = CreateUser();
        var createdAt = DateTimeOffset.UtcNow;
        var token = user.IssueRefreshToken(Guid.NewGuid(), "hash", createdAt.AddMinutes(1), createdAt, Guid.NewGuid(), Guid.NewGuid(), ClientType.Web);

        token.IsExpired(createdAt.AddMinutes(2)).ShouldBeTrue();
        token.IsActive(createdAt.AddMinutes(2)).ShouldBeFalse();
    }

    private static User CreateUser()
    {
        var email = Email.Create("user@example.com").Value;
        var password = PasswordHash.Create(new string('x', PasswordHash.MinLength)).Value;
        var name = FullName.Create("Test", "User").Value;
        return User.Register(Guid.NewGuid(), email, password, name).Value;
    }
}
