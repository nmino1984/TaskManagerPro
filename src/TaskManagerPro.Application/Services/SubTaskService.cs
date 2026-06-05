using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.SubTask;
using TaskManagerPro.Application.Exceptions;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;
namespace TaskManagerPro.Application.Services;

public class SubTaskService : ISubTaskService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public SubTaskService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<SubTaskResponseDto>> GetByTaskAsync(int taskId, string userId)
    {
        var taskExists = await _uow.Tasks.ExistsForUserAsync(taskId, userId);

        if (!taskExists)
        {
            throw new NotFoundException("MyTask", taskId);
        }

        var subtasks = await _uow.SubTasks.GetByTaskIdAsync(taskId);

        return _mapper.Map<List<SubTaskResponseDto>>(subtasks);
    }

    public async Task<SubTaskResponseDto> GetByIdAsync(int id, string userId)
    {
        var subtask = await _uow.SubTasks.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException("SubTask", id);

        return _mapper.Map<SubTaskResponseDto>(subtask);
    }

    public async Task<SubTaskResponseDto> CreateAsync(SubTaskCreateDto dto, string userId)
    {
        var task = await _uow.Tasks.GetByIdAsync(dto.TaskId)
            ?? throw new NotFoundException("MyTask", dto.TaskId);

        if (task.UserId != userId)
        {
            throw new NotFoundException("MyTask", dto.TaskId);
        }

        var subtask = _mapper.Map<SubTask>(dto);
        subtask.CreatedAt = DateTime.UtcNow;
        subtask.UpdatedAt = DateTime.UtcNow;

        await _uow.SubTasks.AddAsync(subtask);
        await _uow.SaveChangesAsync();

        await WriteAuditLogsAsync(subtask.SubTaskId, userId, "Created",
            new List<(string, string?, string?)> { ("Description", null, subtask.Description) });
        await _uow.SaveChangesAsync();

        await SyncTaskProgressAsync(dto.TaskId);

        return _mapper.Map<SubTaskResponseDto>(subtask);
    }

    public async Task<SubTaskResponseDto> UpdateAsync(int id, SubTaskUpdateDto dto, string userId)
    {
        var subtask = await _uow.SubTasks.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException("SubTask", id);

        var oldDescription = subtask.Description;
        var oldStatus = subtask.Status;

        _mapper.Map(dto, subtask);
        subtask.UpdatedAt = DateTime.UtcNow;

        _uow.SubTasks.Update(subtask);

        var changes = new List<(string, string?, string?)>();
        if (oldDescription != subtask.Description)
        {
            changes.Add(("Description", oldDescription, subtask.Description));
        }

        if (oldStatus != subtask.Status)
        {
            changes.Add(("Status", oldStatus.ToString(), subtask.Status.ToString()));
        }

        await WriteAuditLogsAsync(subtask.SubTaskId, userId, "Updated", changes);
        await _uow.SaveChangesAsync();

        await SyncTaskProgressAsync(subtask.TaskId);

        return _mapper.Map<SubTaskResponseDto>(subtask);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var subtask = await _uow.SubTasks.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException("SubTask", id);

        var taskId = subtask.TaskId;

        _uow.SubTasks.Remove(subtask);

        await WriteAuditLogsAsync(subtask.SubTaskId, userId, "Deleted",
            new List<(string, string?, string?)> { ("IsDeleted", "false", "true") });

        await _uow.SaveChangesAsync();

        await SyncTaskProgressAsync(taskId);
    }

    private async Task SyncTaskProgressAsync(int taskId)
    {
        var task = await _uow.Tasks.GetByIdWithSubTasksAsync(taskId);

        if (task is null)
        {
            return;
        }

        task.UpdateProgress();
        task.UpdatedAt = DateTime.UtcNow;
        _uow.Tasks.Update(task);
        await _uow.SaveChangesAsync();
    }

    private async Task WriteAuditLogsAsync(
        int subTaskId,
        string userId,
        string action,
        IReadOnlyCollection<(string FieldName, string? OldValue, string? NewValue)> changes)
    {
        foreach (var (fieldName, oldValue, newValue) in changes)
        {
            var log = new SubTaskAuditLog
            {
                SubTaskId = subTaskId,
                Action = action,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedByUserId = userId,
                ChangedAt = DateTime.UtcNow
            };
            await _uow.SubTaskAuditLogs.AddAsync(log);
        }
    }
}



