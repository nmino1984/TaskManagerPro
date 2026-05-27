namespace MyApp.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    ITaskRepository Tasks { get; }
    ISubTaskRepository SubTasks { get; }
    IMilestoneRepository Milestones { get; }
    INotificationRepository Notifications { get; }
    IUserRepository Users { get; }
    IAuditLogRepository AuditLogs { get; }
    ISubTaskAuditLogRepository SubTaskAuditLogs { get; }
    IMilestoneAuditLogRepository MilestoneAuditLogs { get; }
    ITaskCommentRepository TaskComments { get; }

    Task<int> SaveChangesAsync();
}
