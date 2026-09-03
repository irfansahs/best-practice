using Application.Abstractions.Tenancy;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.Helpers;

internal static class InMemoryDbContextFactory
{
    public static AppDbContext Create(ITenantContext? tenantContext = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options, tenantContext ?? FakeTenantContext.Default);
    }
}
