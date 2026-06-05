using Hangfire;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Infrastructure.Jobs;

namespace TaskManagerPro.Infrastructure.Services;

public class HangfireJobService : IBackgroundJobService
{
    public void EnqueueTaskCreated(int taskId, string userId)
        => BackgroundJob.Enqueue<TaskNotificationJob>(j => j.TaskCreatedAsync(taskId, userId));

    public void EnqueueTaskCompleted(int taskId, string userId)
        => BackgroundJob.Enqueue<TaskNotificationJob>(j => j.TaskCompletedAsync(taskId, userId));

    public void EnqueueTaskAssigned(int taskId, string assignedToUserId)
        => BackgroundJob.Enqueue<TaskNotificationJob>(j => j.TaskAssignedAsync(taskId, assignedToUserId));
}
