using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Domain.Entities;
using TaskManagerPro.Domain.Enums;

namespace TaskManagerPro.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync(u => u.Username == "demo"))
        {
            return;
        }

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Username = "demo",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"),
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(user);

        var tasks = BuildDemoTasks(user.UserId);
        db.MyTasks.AddRange(tasks);

        await db.SaveChangesAsync();
    }

    private static List<MyTask> BuildDemoTasks(string userId) => new()
    {
        new MyTask
        {
            Title = "Diseñar base de datos",
            UserId = userId,
            Priority = TaskPriority.High,
            Status = MyTaskStatus.Completed,
            Progress = 100,
            StartDate = DateTime.UtcNow.AddDays(-20),
            EndDate = DateTime.UtcNow.AddDays(-10),
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            UpdatedAt = DateTime.UtcNow.AddDays(-10)
        },
        new MyTask
        {
            Title = "Implementar API REST",
            UserId = userId,
            Priority = TaskPriority.High,
            Status = MyTaskStatus.InProgress,
            Progress = 60,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(5),
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        },
        new MyTask
        {
            Title = "Escribir tests",
            UserId = userId,
            Priority = TaskPriority.Medium,
            Status = MyTaskStatus.InProgress,
            Progress = 30,
            StartDate = DateTime.UtcNow.AddDays(-5),
            EndDate = DateTime.UtcNow.AddDays(10),
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        },
        new MyTask
        {
            Title = "Despliegue en staging",
            UserId = userId,
            Priority = TaskPriority.Medium,
            Status = MyTaskStatus.NotStarted,
            Progress = 0,
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(15),
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        },
        new MyTask
        {
            Title = "Revisión de seguridad",
            UserId = userId,
            Priority = TaskPriority.High,
            Status = MyTaskStatus.NotStarted,
            Progress = 0,
            StartDate = DateTime.UtcNow.AddDays(8),
            EndDate = DateTime.UtcNow.AddDays(20),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        },
        new MyTask
        {
            Title = "Documentar endpoints",
            UserId = userId,
            Priority = TaskPriority.Low,
            Status = MyTaskStatus.Overdue,
            Progress = 10,
            StartDate = DateTime.UtcNow.AddDays(-15),
            EndDate = DateTime.UtcNow.AddDays(-2),
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        }
    };
}
