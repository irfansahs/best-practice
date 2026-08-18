using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.IntegrationTests.Infrastructure;
using Shouldly;

namespace Api.IntegrationTests.Identity;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AuthEndpointsTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOk()
    {
        if (!IsReady) return;

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@local.dev",
            password = "ChangeMe123!",
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.ShouldContain("accessToken");
        body.ShouldContain("refreshToken");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        if (!IsReady) return;

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@local.dev",
            password = "wrong-password",
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_WithToken_ReturnsOk()
    {
        if (!IsReady) return;

        var token = await IntegrationTestAuth.LoginAsAdminAsync(Client);
        IntegrationTestAuth.SetBearerToken(Client, token);

        var response = await Client.GetAsync("/api/v1/auth/me");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Refresh_WithBodyToken_ReturnsNewTokens_AndOldRefreshFails()
    {
        if (!IsReady) return;

        var login = await LoginAsync();
        var refreshResponse = await Client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken = login.RefreshToken,
        });

        refreshResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var refreshed = await ReadAuthPayloadAsync(refreshResponse);
        refreshed.AccessToken.ShouldNotBeNullOrWhiteSpace();
        refreshed.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        refreshed.RefreshToken.ShouldNotBe(login.RefreshToken);

        var reuseResponse = await Client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken = login.RefreshToken,
        });
        reuseResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithBodyToken_RevokesRefresh()
    {
        if (!IsReady) return;

        var login = await LoginAsync();

        var logoutResponse = await Client.PostAsJsonAsync("/api/v1/auth/logout", new
        {
            refreshToken = login.RefreshToken,
        });
        logoutResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var refreshResponse = await Client.PostAsJsonAsync("/api/v1/auth/refresh", new
        {
            refreshToken = login.RefreshToken,
        });
        refreshResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_WithValidCurrentPassword_AllowsLoginWithNewPassword()
    {
        if (!IsReady) return;

        const string newPassword = "NewSecurePass123!";
        var token = await IntegrationTestAuth.LoginAsAdminAsync(Client);
        IntegrationTestAuth.SetBearerToken(Client, token);

        var changeResponse = await Client.PostAsJsonAsync("/api/v1/auth/change-password", new
        {
            currentPassword = "ChangeMe123!",
            newPassword,
        });
        changeResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var oldLogin = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@local.dev",
            password = "ChangeMe123!",
        });
        oldLogin.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var newLogin = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@local.dev",
            password = newPassword,
        });
        newLogin.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    private async Task<AuthData> LoginAsync()
    {
        var loginResponse = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@local.dev",
            password = "ChangeMe123!",
        });
        loginResponse.EnsureSuccessStatusCode();
        return await ReadAuthPayloadAsync(loginResponse);
    }

    private static async Task<AuthData> ReadAuthPayloadAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<AuthPayload>(JsonOptions);
        payload.ShouldNotBeNull();
        payload.Data.ShouldNotBeNull();
        return payload.Data;
    }

    private sealed record AuthPayload(AuthData Data);

    private sealed record AuthData(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
}
