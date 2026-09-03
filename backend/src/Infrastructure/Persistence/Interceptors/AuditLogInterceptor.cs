using System.Text.Json;
using Application.Abstractions.Security;
using Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public sealed class AuditLogInterceptor(
    ICurrentUser currentUser,
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        WriteAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        WriteAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void WriteAuditLogs(DbContext? context)
    {
        if (context is null) return;

        var auditLogs = context.Set<AuditLog>();
        var userId = currentUser.UserId?.ToString();
        var timestamp = timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries().ToList())
        {
            if (entry.Entity is AuditLog || entry.State is EntityState.Detached or EntityState.Unchanged) continue;
            if (entry.Metadata.IsOwned() || entry.Metadata.FindProperty("Id") is null) continue;

            var entityType = entry.Entity.GetType().Name;
            var entityId = entry.Property("Id").CurrentValue?.ToString() ?? string.Empty;
            var action = entry.State switch
            {
                EntityState.Added => "Added",
                EntityState.Modified => "Modified",
                EntityState.Deleted => "Deleted",
                _ => "Unknown"
            };

            auditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                OldValues = entry.State is EntityState.Modified or EntityState.Deleted ? SerializeValues(entry.OriginalValues) : null,
                NewValues = entry.State is EntityState.Added or EntityState.Modified ? SerializeValues(entry.CurrentValues) : null,
                UserId = userId,
                OrganizationId = currentUser.OrganizationId,
                ActorUserId = currentUser.UserId,
                IsImpersonated = currentUser.IsImpersonating,
                ClientType = currentUser.ClientType?.ToString(),
                CorrelationId = httpContextAccessor.HttpContext?.TraceIdentifier,
                Timestamp = timestamp
            });
        }
    }

    private static string SerializeValues(PropertyValues values)
    {
        var dictionary = values.Properties
            .ToDictionary(p => p.Name, p => values[p]?.ToString());
        return JsonSerializer.Serialize(dictionary, JsonOptions);
    }
}
