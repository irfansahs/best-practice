using System.Net;
using System.Net.Http.Json;
using Api.IntegrationTests.Infrastructure;
using Infrastructure.Persistence.Seed;
using Shouldly;

namespace Api.IntegrationTests.Catalog;

[Collection(nameof(IntegrationTestCollection))]
public sealed class CategoryEndpointsTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task GetCategories_WithAuth_ReturnsSeededCategory()
    {
        if (!IsReady) return;

        var token = await IntegrationTestAuth.LoginAsAdminAsync(Client);
        IntegrationTestAuth.SetBearerToken(Client, token);

        var response = await Client.GetAsync("/api/v1/catalog/categories");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.ShouldContain("General");
    }

    [Fact]
    public async Task CreateCategory_WithValidPayload_ReturnsCreated()
    {
        if (!IsReady) return;

        var token = await IntegrationTestAuth.LoginAsAdminAsync(Client);
        IntegrationTestAuth.SetBearerToken(Client, token);

        var response = await Client.PostAsJsonAsync("/api/v1/catalog/categories", new
        {
            parentCategoryId = (Guid?)null,
            languageId = LanguageSeeder.EnglishId,
            name = "Integration Category",
            description = "Test category",
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DeleteCategory_WithProducts_ReturnsConflict()
    {
        if (!IsReady) return;

        var token = await IntegrationTestAuth.LoginAsAdminAsync(Client);
        IntegrationTestAuth.SetBearerToken(Client, token);

        var response = await Client.DeleteAsync($"/api/v1/catalog/categories/{CatalogSeeder.SampleCategoryId}");
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
