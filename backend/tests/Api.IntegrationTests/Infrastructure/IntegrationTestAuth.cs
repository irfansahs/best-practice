using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.IntegrationTests.Infrastructure;

internal static class IntegrationTestAuth
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static async Task<string> LoginAsAdminAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@local.dev",
            password = "ChangeMe123!",
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<LoginPayload>(JsonOptions);
        return payload!.Data.AccessToken;
    }

    public static void SetBearerToken(HttpClient client, string accessToken) =>
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    private sealed record LoginPayload(LoginData Data);

    private sealed record LoginData(string AccessToken);
}
