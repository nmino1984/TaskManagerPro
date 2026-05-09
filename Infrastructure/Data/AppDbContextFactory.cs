using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyApp.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Hardcoded connection string ONLY for design-time
        optionsBuilder.UseSqlite("Data Source=TaskManagerPro.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}
