using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyApp.Domain.Entities;
using MyApp.Domain.Enums;

namespace MyApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<MyTask> MyTasks { get; set; }
    public DbSet<SubTask> SubTasks { get; set; }
    public DbSet<Milestone> Milestones { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<MyTask>()
            .HasQueryFilter(t => !t.IsDeleted)
            .HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MyTask>()
            .HasOne(t => t.AssignedToUser)
            .WithMany()
            .HasForeignKey(t => t.AssignedToUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<MyTask>()
            .HasMany(t => t.SubTasks)
            .WithOne()
            .HasForeignKey(s => s.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MyTask>()
            .HasMany(t => t.Milestones)
            .WithOne(m => m.Task)
            .IsRequired(false)
            .HasForeignKey(m => m.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MyTask>()
            .Property(t => t.Priority)
            .HasConversion(new EnumToStringConverter<TaskPriority>());

        modelBuilder.Entity<MyTask>()
            .Property(t => t.Status)
            .HasConversion(new EnumToStringConverter<MyTaskStatus>());

        modelBuilder.Entity<MyTask>()
            .HasIndex(t => new { t.UserId, t.IsDeleted, t.CreatedAt })
            .HasDatabaseName("IX_MyTasks_UserId_IsDeleted_CreatedAt");

        modelBuilder.Entity<MyTask>()
            .HasIndex(t => t.Status)
            .HasDatabaseName("IX_MyTasks_Status");

        modelBuilder.Entity<MyTask>()
            .HasIndex(t => t.Priority)
            .HasDatabaseName("IX_MyTasks_Priority");

        modelBuilder.Entity<SubTask>()
            .Property(s => s.Status)
            .HasConversion(new EnumToStringConverter<SubTaskStatus>());

        modelBuilder.Entity<Milestone>()
            .Property(m => m.Status)
            .HasConversion(new EnumToStringConverter<MilestoneStatus>());

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.RelatedTask)
            .WithMany()
            .HasForeignKey(n => n.RelatedTaskId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt })
            .HasDatabaseName("IX_Notifications_UserId_IsRead_CreatedAt");
    }
}
