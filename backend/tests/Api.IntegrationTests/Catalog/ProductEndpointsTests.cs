using System.Net;
using System.Net.Http.Json;
using Api.IntegrationTests.Infrastructure;
using Infrastructure.Persistence.Seed;
using Shouldly;

namespace Api.IntegrationTests.Catalog;

[Collection(nameof(IntegrationTestCollection))]
public sealed class ProductEndpointsTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task GetProductById_WithoutAuth_ReturnsUnauthorized()
    {
        if (!IsReady) return;

        var response = await Client.GetAsync($"/api/v1/catalog/products/{Guid.NewGuid()}");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProducts_WithoutAuth_ReturnsUnauthorized()
    {
        if (!IsReady) return;

        var response = await Client.GetAsync("/api/v1/catalog/products");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProducts_WithAuth_ReturnsSeededProduct()
    {
        if (!IsReady) return;

        var token = await IntegrationTestAuth.LoginAsAdminAsync(Client);
        IntegrationTestAuth.SetBearerToken(Client, token);

        var response = await Client.GetAsync("/api/v1/catalog/products?page=1&pageSize=10");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.ShouldContain("SAMPLE-001");
    }

    [Fact]
    public async Task CreateProduct_WithValidPayload_ReturnsCreated()
    {
        if (!IsReady) return;

        var token = await IntegrationTestAuth.LoginAsAdminAsync(Client);
        IntegrationTestAuth.SetBearerToken(Client, token);

        var sku = $"IT-{Guid.NewGuid():N}"[..12];
        var response = await Client.PostAsJsonAsync("/api/v1/catalog/products", new
        {
            sku,
            price = 12.5m,
            currency = "USD",
            categoryId = CatalogSeeder.SampleCategoryId,
            languageId = LanguageSeeder.EnglishId,
            name = "Integration Product",
            description = "Created in integration test",
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateProduct_WithEmptySku_ReturnsBadRequest()
    {
        if (!IsReady) return;

        var token = await IntegrationTestAuth.LoginAsAdminAsync(Client);
        IntegrationTestAuth.SetBearerToken(Client, token);

        var response = await Client.PostAsJsonAsync("/api/v1/catalog/products", new
        {
            sku = "",
            price = 1m,
            currency = "USD",
            categoryId = CatalogSeeder.SampleCategoryId,
            languageId = LanguageSeeder.EnglishId,
            name = "Bad Product",
        });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
