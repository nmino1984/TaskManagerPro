using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagerPro.Infrastructure.Persistence;

namespace TaskManagerPro.Integration.Tests.Infrastructure;

// Each factory instance owns a single open SQLite :memory: connection.
// Passing the connection object (not a string) to UseSqlite ensures all
// requests share the same in-memory database for the lifetime of the factory.
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;
    private bool _dbInitialized;

    public CustomWebApplicationFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        if (_dbInitialized) return;
        _dbInitialized = true;

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}
