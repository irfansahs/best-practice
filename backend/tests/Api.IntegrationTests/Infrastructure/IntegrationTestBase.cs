using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests.Infrastructure;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly IServiceScope? _scope;
    private IDbContextTransaction? _transaction;

    protected IntegrationTestBase(DatabaseFixture fixture)
    {
        Fixture = fixture;

        if (!fixture.IsDockerAvailable || fixture.Factory is null)
        {
            Client = null!;
            DbContext = null!;
            return;
        }

        Client = fixture.Factory.CreateClient();
        _scope = fixture.Factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    protected DatabaseFixture Fixture { get; }

    protected HttpClient Client { get; }

    protected AppDbContext DbContext { get; }

    protected bool IsReady => Fixture.IsDockerAvailable && Fixture.Factory is not null;

    public async Task InitializeAsync()
    {
        if (!IsReady)
            return;

        _transaction = await DbContext.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        _scope?.Dispose();
    }
}

[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection : ICollectionFixture<DatabaseFixture>;
