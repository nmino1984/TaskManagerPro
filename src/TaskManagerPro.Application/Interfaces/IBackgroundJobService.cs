namespace TaskManagerPro.Application.Interfaces;

public interface IBackgroundJobService
{
    void EnqueueTaskCreated(int taskId, string userId);
    void EnqueueTaskCompleted(int taskId, string userId);
    void EnqueueTaskAssigned(int taskId, string assignedToUserId);
}
