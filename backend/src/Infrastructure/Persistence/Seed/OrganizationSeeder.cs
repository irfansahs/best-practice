using Domain.Tenancy;
using Domain.Tenancy.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class OrganizationSeeder
{
    public static readonly Guid RannaId = Guid.Parse("11111111-1111-1111-1111-111111111101");
    public static readonly Guid AquaCareId = Guid.Parse("11111111-1111-1111-1111-111111111102");

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Organizations.IgnoreQueryFilters().AnyAsync(cancellationToken))
            return;

        var ranna = Organization.CreateRoot(
            RannaId,
            "Ranna",
            OrganizationSlug.Create("ranna").Value).Value;

        var aquacare = Organization.CreateChild(
            AquaCareId,
            ranna,
            "AquaCare",
            OrganizationSlug.Create("aquacare").Value).Value;

        context.Organizations.AddRange(ranna, aquacare);
        await context.SaveChangesAsync(cancellationToken);
    }
}
