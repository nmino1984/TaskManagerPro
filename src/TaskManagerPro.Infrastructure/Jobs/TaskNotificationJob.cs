using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Domain.Enums;
using TaskManagerPro.Infrastructure.Persistence;

namespace TaskManagerPro.Infrastructure.Jobs;

public class TaskNotificationJob
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TaskNotificationJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task TaskCreatedAsync(int taskId, string userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var task = await db.MyTasks
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.MyTaskId == taskId && t.UserId == userId);

        if (task == null)
        {
            return;
        }

        await notificationService.CreateAsync(
            userId,
            "Task Created",
            $"Task '{task.Title}' has been created",
            "TaskCreated",
            taskId
        );
    }

    public async Task TaskCompletedAsync(int taskId, string userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var task = await db.MyTasks
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.MyTaskId == taskId && t.UserId == userId);

        if (task == null)
        {
            return;
        }

        await notificationService.CreateAsync(
            userId,
            "Task Completed",
            $"Task '{task.Title}' has been marked as completed",
            "TaskCompleted",
            taskId
        );
    }

    public async Task TaskAssignedAsync(int taskId, string assignedToUserId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var task = await db.MyTasks
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.MyTaskId == taskId);

        if (task == null)
        {
            return;
        }

        if (task.UserId == assignedToUserId)
        {
            return;
        }

        await notificationService.CreateAsync(
            assignedToUserId,
            "Task Assigned",
            $"Task '{task.Title}' has been assigned to you",
            "TaskAssigned",
            taskId
        );
    }

    public async Task TaskOverdueCheckAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var now = DateTime.UtcNow;
        var overdueTasks = await db.MyTasks
            .IgnoreQueryFilters()
            .Where(t => !t.IsDeleted && t.Status != MyTaskStatus.Completed && t.EndDate < now)
            .ToListAsync();

        foreach (var task in overdueTasks)
        {
            var existingNotification = await db.Notifications
                .FirstOrDefaultAsync(n => n.RelatedTaskId == task.MyTaskId && n.Type == "TaskOverdue");

            if (existingNotification == null)
            {
                await notificationService.CreateAsync(
                    task.UserId,
                    "Task Overdue",
                    $"Task '{task.Title}' is overdue",
                    "TaskOverdue",
                    task.MyTaskId
                );

                task.Status = MyTaskStatus.Overdue;
            }
        }

        await db.SaveChangesAsync();
    }
}
