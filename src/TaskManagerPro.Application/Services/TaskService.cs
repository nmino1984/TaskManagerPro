using System;
using AutoMapper;
using TaskManagerPro.Application.DTOs.Common;
using TaskManagerPro.Application.DTOs.MyTask;
using TaskManagerPro.Application.Exceptions;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;
using TaskManagerPro.Domain.Enums;

using System.Linq;
using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs;
using TaskManagerPro.Application.Interfaces;
using System.Collections.Generic;
namespace TaskManagerPro.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IBackgroundJobService _jobs;

    public TaskService(IUnitOfWork uow, IMapper mapper, IBackgroundJobService jobs)
    {
        _uow = uow;
        _mapper = mapper;
        _jobs = jobs;
    }

    public async Task<PagedResult<MyTaskResponseDto>> GetAllAsync(string userId, TaskQueryParams q)
    {
        var pagedResult = await _uow.Tasks.GetPagedAsync(userId, q);

        return new PagedResult<MyTaskResponseDto>
        {
            Items = _mapper.Map<List<MyTaskResponseDto>>(pagedResult.Items),
            TotalCount = pagedResult.TotalCount,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize
        };
    }

    public async Task<MyTaskResponseDto> GetByIdAsync(int id, string userId)
    {
        var task = await _uow.Tasks.GetByIdWithIncludesAsync(id, userId)
            ?? throw new NotFoundException("MyTask", id);

        return _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task<MyTaskResponseDto> CreateAsync(MyTaskCreateDto dto, string userId)
    {
        var task = _mapper.Map<MyTask>(dto);
        task.UserId = userId;
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        await _uow.Tasks.AddAsync(task);
        await _uow.SaveChangesAsync();

        await WriteAuditLogsAsync(task.MyTaskId, userId, "Created",
            new List<(string, string?, string?)> { ("Title", null, task.Title) });
        await _uow.SaveChangesAsync();

        _jobs.EnqueueTaskCreated(task.MyTaskId, userId);

        return _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task<MyTaskResponseDto> UpdateAsync(int id, MyTaskUpdateDto dto, string userId)
    {
        var task = await _uow.Tasks.GetByIdWithIncludesAsync(id, userId)
            ?? throw new NotFoundException("MyTask", id);

        var oldTitle = task.Title;
        var oldDescription = task.Description;
        var oldStartDate = task.StartDate;
        var oldEndDate = task.EndDate;
        var oldPriority = task.Priority;
        var oldStatus = task.Status;
        var oldAssignedToUserId = task.AssignedToUserId;

        _mapper.Map(dto, task);
        task.UpdatedAt = DateTime.UtcNow;

        _uow.Tasks.Update(task);

        var changes = new List<(string, string?, string?)>();
        if (oldTitle != task.Title)
            changes.Add(("Title", oldTitle, task.Title));
        if (oldDescription != task.Description)
            changes.Add(("Description", oldDescription, task.Description));
        if (oldStartDate != task.StartDate)
            changes.Add(("StartDate", oldStartDate.ToString("O"), task.StartDate.ToString("O")));
        if (oldEndDate != task.EndDate)
            changes.Add(("EndDate", oldEndDate.ToString("O"), task.EndDate.ToString("O")));
        if (oldPriority != task.Priority)
            changes.Add(("Priority", oldPriority.ToString(), task.Priority.ToString()));
        if (oldStatus != task.Status)
            changes.Add(("Status", oldStatus.ToString(), task.Status.ToString()));
        if (oldAssignedToUserId != task.AssignedToUserId)
            changes.Add(("AssignedToUserId", oldAssignedToUserId, task.AssignedToUserId));

        await WriteAuditLogsAsync(task.MyTaskId, userId, "Updated", changes);
        await _uow.SaveChangesAsync();

        if (oldStatus != MyTaskStatus.Completed && task.Status == MyTaskStatus.Completed)
            _jobs.EnqueueTaskCompleted(task.MyTaskId, userId);

        if (oldAssignedToUserId != task.AssignedToUserId && !string.IsNullOrEmpty(task.AssignedToUserId))
            _jobs.EnqueueTaskAssigned(task.MyTaskId, task.AssignedToUserId);

        return _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var task = await _uow.Tasks.GetByIdWithIncludesAsync(id, userId)
            ?? throw new NotFoundException("MyTask", id);

        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;
        _uow.Tasks.Update(task);

        await WriteAuditLogsAsync(task.MyTaskId, userId, "Deleted",
            new List<(string, string?, string?)> { ("IsDeleted", "false", "true") });

        await _uow.SaveChangesAsync();
    }

    public async Task<TaskAssignmentResponseDto> AssignAsync(int id, AssignTaskDto dto, string userId)
    {
        var task = await _uow.Tasks.GetByIdWithIncludesAsync(id, userId)
            ?? throw new NotFoundException("MyTask", id);

        if (task.UserId != userId)
            throw new UnauthorizedAccessException("You can only assign tasks you own.");

        var oldAssignedToUserId = task.AssignedToUserId;

        if (!string.IsNullOrEmpty(dto.AssignToUserId))
        {
            var assignedUser = await _uow.Users.GetByIdAsync(dto.AssignToUserId)
                ?? throw new NotFoundException($"User with ID {dto.AssignToUserId} was not found.");

            task.AssignedToUserId = dto.AssignToUserId;
            task.UpdatedAt = DateTime.UtcNow;
            _uow.Tasks.Update(task);

            await WriteAuditLogsAsync(task.MyTaskId, userId, "Updated",
                new List<(string, string?, string?)> { ("AssignedToUserId", oldAssignedToUserId, task.AssignedToUserId) });
            await _uow.SaveChangesAsync();

            _jobs.EnqueueTaskAssigned(task.MyTaskId, dto.AssignToUserId);

            return new TaskAssignmentResponseDto
            {
                MyTaskId = task.MyTaskId,
                Title = task.Title,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUsername = assignedUser.Username
            };
        }

        task.AssignedToUserId = null;
        task.UpdatedAt = DateTime.UtcNow;
        _uow.Tasks.Update(task);

        await WriteAuditLogsAsync(task.MyTaskId, userId, "Updated",
            new List<(string, string?, string?)> { ("AssignedToUserId", oldAssignedToUserId, null) });
        await _uow.SaveChangesAsync();

        return new TaskAssignmentResponseDto
        {
            MyTaskId = task.MyTaskId,
            Title = task.Title,
            AssignedToUserId = null,
            AssignedToUsername = null
        };
    }

    private async Task WriteAuditLogsAsync(
        int taskId,
        string userId,
        string action,
        IReadOnlyCollection<(string FieldName, string? OldValue, string? NewValue)> changes)
    {
        foreach (var (fieldName, oldValue, newValue) in changes)
        {
            var log = new TaskAuditLog
            {
                TaskId = taskId,
                Action = action,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedByUserId = userId,
                ChangedAt = DateTime.UtcNow
            };
            await _uow.AuditLogs.AddAsync(log);
        }
    }
}



