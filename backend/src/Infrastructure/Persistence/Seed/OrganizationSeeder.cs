using Domain.Tenancy;
using Domain.Tenancy.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class OrganizationSeeder
{
    public static readonly Guid RannaId = Guid.Parse("11111111-1111-1111-1111-111111111101");
    public static readonly Guid AquaCareId = Guid.Parse("11111111-1111-1111-1111-111111111102");
    public static readonly Guid DemoSupplierId = Guid.Parse("11111111-1111-1111-1111-111111111103");

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        var ranna = await context.Organizations.IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Id == RannaId, cancellationToken);

        if (ranna is null)
        {
            ranna = Organization.CreateRoot(
                RannaId,
                "Ranna",
                OrganizationSlug.Create("ranna").Value).Value;
            context.Organizations.Add(ranna);
            await context.SaveChangesAsync(cancellationToken);
        }

        var aquacare = await context.Organizations.IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Id == AquaCareId, cancellationToken);

        if (aquacare is null)
        {
            aquacare = Organization.CreateChild(
                AquaCareId,
                ranna,
                "AquaCare",
                OrganizationSlug.Create("aquacare").Value).Value;
            context.Organizations.Add(aquacare);
            await context.SaveChangesAsync(cancellationToken);
        }

        if (await context.Organizations.IgnoreQueryFilters().AnyAsync(o => o.Id == DemoSupplierId, cancellationToken))
            return;

        var supplier = Organization.CreateChild(
            DemoSupplierId,
            aquacare,
            "Demo Supplier",
            OrganizationSlug.Create("demo-supplier").Value).Value;
        context.Organizations.Add(supplier);
        await context.SaveChangesAsync(cancellationToken);
    }
}
