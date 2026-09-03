using Api.Extensions;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddInfrastructureSerilog();
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.ConfigurePipeline();

if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
        await app.Services.SeedDatabaseAsync();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Database migration or seeding failed during startup");
        throw;
    }
}
else
{
    Log.Information(
        "Skipping automatic Migrate/Seed outside Development. Apply with: " +
        "dotnet ef database update --project backend/src/Infrastructure --startup-project backend/src/Api");
}

app.Run();
