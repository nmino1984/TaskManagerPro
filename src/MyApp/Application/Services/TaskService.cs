using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.Common;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.Exceptions;
using MyApp.Application.Interfaces;
using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;
using MyApp.Domain.Enums;
using MyApp.Infrastructure.Jobs;

namespace MyApp.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public TaskService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
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

        BackgroundJob.Enqueue<TaskNotificationJob>(j => j.TaskCreatedAsync(task.MyTaskId, userId));

        return _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task<MyTaskResponseDto> UpdateAsync(int id, MyTaskUpdateDto dto, string userId)
    {
        var task = await _uow.Tasks.GetByIdWithIncludesAsync(id, userId)
            ?? throw new NotFoundException("MyTask", id);

        var oldStatus = task.Status;
        var oldAssignedToUserId = task.AssignedToUserId;

        _mapper.Map(dto, task);
        task.UpdatedAt = DateTime.UtcNow;

        _uow.Tasks.Update(task);
        await _uow.SaveChangesAsync();

        if (oldStatus != MyTaskStatus.Completed && task.Status == MyTaskStatus.Completed)
            BackgroundJob.Enqueue<TaskNotificationJob>(j => j.TaskCompletedAsync(task.MyTaskId, userId));

        if (oldAssignedToUserId != task.AssignedToUserId && !string.IsNullOrEmpty(task.AssignedToUserId))
            BackgroundJob.Enqueue<TaskNotificationJob>(j => j.TaskAssignedAsync(task.MyTaskId, task.AssignedToUserId));

        return _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var task = await _uow.Tasks.GetByIdWithIncludesAsync(id, userId)
            ?? throw new NotFoundException("MyTask", id);

        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;
        _uow.Tasks.Update(task);
        await _uow.SaveChangesAsync();
    }

    public async Task<TaskAssignmentResponseDto> AssignAsync(int id, AssignTaskDto dto, string userId)
    {
        var task = await _uow.Tasks.GetByIdWithIncludesAsync(id, userId)
            ?? throw new NotFoundException("MyTask", id);

        if (task.UserId != userId)
            throw new UnauthorizedAccessException("You can only assign tasks you own.");

        if (!string.IsNullOrEmpty(dto.AssignToUserId))
        {
            var assignedUser = await _uow.Users.Query()
                .FirstOrDefaultAsync(u => u.UserId == dto.AssignToUserId)
                ?? throw new NotFoundException($"User with ID {dto.AssignToUserId} was not found.");

            task.AssignedToUserId = dto.AssignToUserId;
            task.UpdatedAt = DateTime.UtcNow;
            _uow.Tasks.Update(task);
            await _uow.SaveChangesAsync();

            BackgroundJob.Enqueue<TaskNotificationJob>(j => j.TaskAssignedAsync(task.MyTaskId, dto.AssignToUserId));

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
        await _uow.SaveChangesAsync();

        return new TaskAssignmentResponseDto
        {
            MyTaskId = task.MyTaskId,
            Title = task.Title,
            AssignedToUserId = null,
            AssignedToUsername = null
        };
    }
}
