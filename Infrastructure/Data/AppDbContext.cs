using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<MyTask> MyTasks { get; set; }
    public DbSet<SubTask> SubTasks { get; set; }
    public DbSet<CalendarEvent> CalendarEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MyTask>()
            .HasMany(t => t.SubTasks)
            .WithOne()
            .HasForeignKey(s => s.TaskId);

        modelBuilder.Entity<CalendarEvent>()
            .HasOne(e => e.Task)
            .WithMany()
            .HasForeignKey(e => e.TaskId);
    }
}
