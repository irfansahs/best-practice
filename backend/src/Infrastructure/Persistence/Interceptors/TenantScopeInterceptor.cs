using Application.Abstractions.Tenancy;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public sealed class TenantScopeInterceptor(ITenantContext tenantContext) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries<ITenantScoped>())
        {
            if (entry.State != EntityState.Added) continue;
            if (entry.Entity.OrganizationId != Guid.Empty && !string.IsNullOrWhiteSpace(entry.Entity.OrganizationPath))
                continue;

            if (!tenantContext.IsAvailable)
                throw new InvalidOperationException("Tenant context is required to persist tenant-scoped entities.");

            entry.Entity.AssignTenant(tenantContext.OrganizationId, tenantContext.OrganizationPath);
        }
    }
}
