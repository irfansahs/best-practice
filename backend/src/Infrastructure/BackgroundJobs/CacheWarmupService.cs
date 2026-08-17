using Application.Abstractions.Localization;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

public sealed class CacheWarmupService(IServiceScopeFactory serviceScopeFactory, ILogger<CacheWarmupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await WarmupTranslationsAsync(stoppingToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Cache warmup failed.");
        }
    }

    private async Task WarmupTranslationsAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var cultures = await context.Languages.AsNoTracking()
            .Where(l => l.IsActive)
            .OrderBy(l => l.SortOrder)
            .Select(l => l.Code)
            .ToListAsync(cancellationToken);

        var translationProvider = scope.ServiceProvider.GetRequiredService<ITranslationProvider>();
        foreach (var culture in cultures)
        {
            await translationProvider.GetResourcesAsync(culture, cancellationToken: cancellationToken);
            logger.LogInformation("Warmed translation cache for culture {Culture}", culture);
        }
    }
}
