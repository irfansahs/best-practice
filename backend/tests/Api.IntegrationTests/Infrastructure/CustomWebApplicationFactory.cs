using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Api.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ConnectionString"] = connectionString,
                ["Database:CommandTimeout"] = "30",
                ["Database:MaxRetryCount"] = "0",
                ["Database:MaxRetryDelay"] = "00:00:05",
                ["Database:SlowQueryThresholdMs"] = "1000",
            });
        });
    }
}
