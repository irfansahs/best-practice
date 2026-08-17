using Testcontainers.MsSql;

namespace Api.IntegrationTests.Infrastructure;

public sealed class DatabaseFixture : IAsyncLifetime
{
    private MsSqlContainer? _container;

    public bool IsDockerAvailable { get; private set; }

    public string? ConnectionString { get; private set; }

    public CustomWebApplicationFactory? Factory { get; private set; }

    public async Task InitializeAsync()
    {
        try
        {
            _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
            await _container.StartAsync();
            ConnectionString = _container.GetConnectionString();
            Factory = new CustomWebApplicationFactory(ConnectionString);
            IsDockerAvailable = true;
        }
        catch (Exception)
        {
            IsDockerAvailable = false;
        }
    }

    public async Task DisposeAsync()
    {
        if (Factory is not null)
            await Factory.DisposeAsync();

        if (_container is not null)
            await _container.DisposeAsync();
    }
}
