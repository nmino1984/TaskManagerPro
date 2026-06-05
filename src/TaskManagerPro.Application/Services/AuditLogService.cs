using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.AuditLog;
using TaskManagerPro.Application.Exceptions;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Application.Interfaces.Repositories;
namespace TaskManagerPro.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    // TODO: paginate history endpoints — long-lived tasks end up with hundreds of log entries
    // TODO: add date range filter so clients can scope history to a sprint

    public AuditLogService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TaskAuditLogResponseDto>> GetTaskHistoryAsync(int taskId, string userId)
    {
        var task = await _uow.Tasks.GetByIdAsync(taskId)
            ?? throw new NotFoundException("MyTask", taskId);

        if (task.UserId != userId)
        {
            throw new NotFoundException("MyTask", taskId);
        }

        var logs = await _uow.AuditLogs.GetByTaskIdAsync(taskId, userId);
        return _mapper.Map<IEnumerable<TaskAuditLogResponseDto>>(logs);
    }

    public async Task<IEnumerable<SubTaskAuditLogResponseDto>> GetSubTaskHistoryAsync(int subTaskId, string userId)
    {
        var subtask = await _uow.SubTasks.GetByIdAsync(subTaskId)
            ?? throw new NotFoundException("SubTask", subTaskId);

        var task = await _uow.Tasks.GetByIdAsync(subtask.TaskId)
            ?? throw new NotFoundException("MyTask", subtask.TaskId);

        if (task.UserId != userId)
        {
            throw new NotFoundException("SubTask", subTaskId);
        }

        var logs = await _uow.SubTaskAuditLogs.GetBySubTaskIdAsync(subTaskId);
        return _mapper.Map<IEnumerable<SubTaskAuditLogResponseDto>>(logs);
    }

    public async Task<IEnumerable<MilestoneAuditLogResponseDto>> GetMilestoneHistoryAsync(int milestoneId, string userId)
    {
        var milestone = await _uow.Milestones.GetByIdAsync(milestoneId)
            ?? throw new NotFoundException("Milestone", milestoneId);

        var task = await _uow.Tasks.GetByIdAsync(milestone.TaskId)
            ?? throw new NotFoundException("MyTask", milestone.TaskId);

        if (task.UserId != userId)
        {
            throw new NotFoundException("Milestone", milestoneId);
        }

        var logs = await _uow.MilestoneAuditLogs.GetByMilestoneIdAsync(milestoneId);
        return _mapper.Map<IEnumerable<MilestoneAuditLogResponseDto>>(logs);
    }

    // not used yet — placeholder for future "hide noisy field changes" filter
    private bool IsSignificantChange(string fieldName) =>
        fieldName is "Title" or "Status" or "DueDate" or "Priority";
}



