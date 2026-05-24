using MyApp.Application.DTOs.AuditLog;

namespace MyApp.Application.Interfaces;

public interface IAuditLogService
{
    Task<IEnumerable<TaskAuditLogResponseDto>> GetTaskHistoryAsync(int taskId, string userId);
    Task<IEnumerable<SubTaskAuditLogResponseDto>> GetSubTaskHistoryAsync(int subTaskId, string userId);
    Task<IEnumerable<MilestoneAuditLogResponseDto>> GetMilestoneHistoryAsync(int milestoneId, string userId);
}
