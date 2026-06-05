using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskManagerPro.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlite("Data Source=TaskManagerPro.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}
