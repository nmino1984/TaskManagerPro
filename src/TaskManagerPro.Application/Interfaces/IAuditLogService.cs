using System;
using TaskManagerPro.Application.DTOs.AuditLog;

using System.Threading.Tasks;
using System.Collections.Generic;
namespace TaskManagerPro.Application.Interfaces;

public interface IAuditLogService
{
    Task<IEnumerable<TaskAuditLogResponseDto>> GetTaskHistoryAsync(int taskId, string userId);
    Task<IEnumerable<SubTaskAuditLogResponseDto>> GetSubTaskHistoryAsync(int subTaskId, string userId);
    Task<IEnumerable<MilestoneAuditLogResponseDto>> GetMilestoneHistoryAsync(int milestoneId, string userId);
}

