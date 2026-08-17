using System.Net;
using System.Net.Http.Json;
using Api.IntegrationTests.Infrastructure;
using Shouldly;

namespace Api.IntegrationTests.Localization;

[Collection(nameof(IntegrationTestCollection))]
public sealed class LocalizationEndpointsTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task GetLanguages_AllowsAnonymous()
    {
        if (!IsReady) return;

        var response = await Client.GetAsync("/api/v1/localization/languages");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetResources_AllowsAnonymous()
    {
        if (!IsReady) return;

        var response = await Client.GetAsync("/api/v1/localization/resources/en");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpsertTranslation_WithoutAuth_ReturnsUnauthorized()
    {
        if (!IsReady) return;

        var response = await Client.PutAsJsonAsync("/api/v1/localization/translations", new
        {
            languageId = Guid.NewGuid(),
            @namespace = "Common",
            key = "Test.Key",
            value = "Test",
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
