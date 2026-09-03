using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.IntegrationTests.Infrastructure;
using Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Api.IntegrationTests.Tenancy;

[Collection(nameof(IntegrationTestCollection))]
public sealed class TenantIsolationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task SupplierA_CannotReadSupplierB_Product()
    {
        if (!IsReady) return;

        var suffix = Guid.NewGuid().ToString("N")[..8];
        var admin = await LoginAsync("admin@local.dev", "ChangeMe123!");
        SetBearer(admin.AccessToken);

        var supplierAId = await CreateOrganizationAsync($"Supplier A {suffix}", $"supplier-a-{suffix}", OrganizationSeeder.AquaCareId);
        var supplierBId = await CreateOrganizationAsync($"Supplier B {suffix}", $"supplier-b-{suffix}", OrganizationSeeder.AquaCareId);

        var userA = await RegisterAsync($"a-{suffix}@test.dev", "SupplierPass123!");
        var userB = await RegisterAsync($"b-{suffix}@test.dev", "SupplierPass123!");

        await AddMemberAsync(supplierAId, userA, PermissionSeeder.SupplierAdminRoleId);
        await AddMemberAsync(supplierBId, userB, PermissionSeeder.SupplierAdminRoleId);

        var sessionA = await LoginAsync($"a-{suffix}@test.dev", "SupplierPass123!");
        SetBearer(sessionA.AccessToken);
        var categoryA = await CreateCategoryAsync($"Cat A {suffix}");
        var skuA = $"A{suffix}";
        var productAId = await CreateProductAsync(skuA, categoryA);

        var sessionB = await LoginAsync($"b-{suffix}@test.dev", "SupplierPass123!");
        SetBearer(sessionB.AccessToken);
        var categoryB = await CreateCategoryAsync($"Cat B {suffix}");
        var skuB = $"B{suffix}";
        await CreateProductAsync(skuB, categoryB);

        var foreign = await Client.GetAsync($"/api/v1/catalog/products/{productAId}");
        foreign.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var list = await Client.GetAsync("/api/v1/catalog/products?page=1&pageSize=50");
        list.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await list.Content.ReadAsStringAsync();
        body.ShouldContain(skuB);
        body.ShouldNotContain(skuA);
    }

    [Fact]
    public async Task SuspendedOrganization_CannotLogin()
    {
        if (!IsReady) return;

        var suffix = Guid.NewGuid().ToString("N")[..8];
        var admin = await LoginAsync("admin@local.dev", "ChangeMe123!");
        SetBearer(admin.AccessToken);

        var orgId = await CreateOrganizationAsync($"Suspended {suffix}", $"suspended-{suffix}", OrganizationSeeder.AquaCareId);
        var userId = await RegisterAsync($"s-{suffix}@test.dev", "SupplierPass123!");
        await AddMemberAsync(orgId, userId, PermissionSeeder.SupplierAdminRoleId);

        var suspend = await Client.PostAsJsonAsync($"/api/v1/tenancy/organizations/{orgId}/status", new { status = "Suspended" });
        suspend.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var login = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = $"s-{suffix}@test.dev",
            password = "SupplierPass123!",
            clientType = "web",
            organizationId = orgId
        });

        login.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Impersonation_WritesAuditFlag()
    {
        if (!IsReady) return;

        var login = await LoginAsync("admin@local.dev", "ChangeMe123!");
        var switched = await Client.PostAsJsonAsync("/api/v1/auth/switch-organization", new
        {
            organizationId = OrganizationSeeder.AquaCareId,
            refreshToken = login.RefreshToken,
            clientType = "web"
        });
        switched.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tokens = await ReadAuthAsync(switched);
        SetBearer(tokens.AccessToken);

        var sku = $"I{Guid.NewGuid():N}"[..12];
        var create = await Client.PostAsJsonAsync("/api/v1/catalog/products", new
        {
            sku,
            price = 9.5m,
            currency = "USD",
            categoryId = CatalogSeeder.SampleCategoryId,
            languageId = LanguageSeeder.EnglishId,
            name = "Impersonated product"
        });
        create.StatusCode.ShouldBe(HttpStatusCode.Created);

        var audit = await DbContext.AuditLogs
            .AsNoTracking()
            .Where(a => a.EntityType == "Product" && a.IsImpersonated)
            .OrderByDescending(a => a.Timestamp)
            .FirstOrDefaultAsync();

        audit.ShouldNotBeNull();
        audit.IsImpersonated.ShouldBeTrue();
        audit.OrganizationId.ShouldBe(OrganizationSeeder.AquaCareId);
    }

    [Fact]
    public async Task PlatformAdmin_MobileLogin_IsRejected()
    {
        if (!IsReady) return;

        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@local.dev",
            password = "ChangeMe123!",
            clientType = "mobile"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private void SetBearer(string accessToken) => IntegrationTestAuth.SetBearerToken(Client, accessToken);

    private async Task<AuthData> LoginAsync(string email, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email,
            password,
            clientType = "web"
        });
        response.EnsureSuccessStatusCode();
        return await ReadAuthAsync(response);
    }

    private static async Task<AuthData> ReadAuthAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<Envelope<AuthData>>(JsonOptions);
        payload.ShouldNotBeNull();
        payload.Data.ShouldNotBeNull();
        return payload.Data;
    }

    private async Task<Guid> RegisterAsync(string email, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email,
            password,
            firstName = "Test",
            lastName = "User"
        });
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<Envelope<RegisterData>>(JsonOptions);
        return payload!.Data.UserId;
    }

    private async Task<Guid> CreateOrganizationAsync(string name, string slug, Guid parentId)
    {
        var response = await Client.PostAsJsonAsync("/api/v1/tenancy/organizations", new
        {
            name,
            slug,
            parentId
        });
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<Envelope<CreatedId>>(JsonOptions);
        return payload!.Data.Id;
    }

    private async Task AddMemberAsync(Guid organizationId, Guid userId, Guid roleId)
    {
        var response = await Client.PostAsJsonAsync($"/api/v1/tenancy/organizations/{organizationId}/members", new
        {
            userId,
            roleIds = new[] { roleId },
            title = "Admin",
            isPrimary = true
        });
        response.EnsureSuccessStatusCode();
    }

    private async Task<Guid> CreateCategoryAsync(string name)
    {
        var response = await Client.PostAsJsonAsync("/api/v1/catalog/categories", new
        {
            languageId = LanguageSeeder.EnglishId,
            name
        });
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<Envelope<CreatedId>>(JsonOptions);
        return payload!.Data.Id;
    }

    private async Task<Guid> CreateProductAsync(string sku, Guid categoryId)
    {
        var response = await Client.PostAsJsonAsync("/api/v1/catalog/products", new
        {
            sku,
            price = 11m,
            currency = "USD",
            categoryId,
            languageId = LanguageSeeder.EnglishId,
            name = sku
        });
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<Envelope<CreatedId>>(JsonOptions);
        return payload!.Data.Id;
    }

    private sealed record Envelope<T>(T Data);
    private sealed record AuthData(string AccessToken, string RefreshToken);
    private sealed record RegisterData(Guid UserId);
    private sealed record CreatedId(Guid Id);
}
