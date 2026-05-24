using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.SubTask;
using MyApp.Application.Exceptions;
using MyApp.Application.Interfaces;
using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;

namespace MyApp.Application.Services;

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
        var taskExists = await _uow.Tasks.Query()
            .AnyAsync(t => t.MyTaskId == taskId && t.UserId == userId);

        if (!taskExists)
            throw new NotFoundException("MyTask", taskId);

        var subtasks = await _uow.SubTasks.Query()
            .Where(s => s.TaskId == taskId)
            .ToListAsync();

        return _mapper.Map<List<SubTaskResponseDto>>(subtasks);
    }

    public async Task<SubTaskResponseDto> GetByIdAsync(int id, string userId)
    {
        var subtask = await _uow.SubTasks.Query()
            .Where(s => s.SubTaskId == id)
            .Join(_uow.Tasks.Query(), st => st.TaskId, t => t.MyTaskId, (st, t) => new { SubTask = st, Task = t })
            .FirstOrDefaultAsync(x => x.Task.UserId == userId)
            ?? throw new NotFoundException("SubTask", id);

        return _mapper.Map<SubTaskResponseDto>(subtask.SubTask);
    }

    public async Task<SubTaskResponseDto> CreateAsync(SubTaskCreateDto dto, string userId)
    {
        var task = await _uow.Tasks.Query()
            .FirstOrDefaultAsync(t => t.MyTaskId == dto.TaskId)
            ?? throw new NotFoundException("MyTask", dto.TaskId);

        if (task.UserId != userId)
            throw new UnauthorizedAccessException("You do not have access to this task.");

        var subtask = _mapper.Map<SubTask>(dto);
        subtask.CreatedAt = DateTime.UtcNow;
        subtask.UpdatedAt = DateTime.UtcNow;

        await _uow.SubTasks.AddAsync(subtask);
        await _uow.SaveChangesAsync();

        await SyncTaskProgressAsync(dto.TaskId);

        return _mapper.Map<SubTaskResponseDto>(subtask);
    }

    public async Task<SubTaskResponseDto> UpdateAsync(int id, SubTaskUpdateDto dto, string userId)
    {
        var subtask = await _uow.SubTasks.Query()
            .Where(s => s.SubTaskId == id)
            .Join(_uow.Tasks.Query(), st => st.TaskId, t => t.MyTaskId, (st, t) => new { SubTask = st, Task = t })
            .FirstOrDefaultAsync(x => x.Task.UserId == userId)
            ?? throw new NotFoundException("SubTask", id);

        _mapper.Map(dto, subtask.SubTask);
        subtask.SubTask.UpdatedAt = DateTime.UtcNow;

        _uow.SubTasks.Update(subtask.SubTask);
        await _uow.SaveChangesAsync();

        await SyncTaskProgressAsync(subtask.SubTask.TaskId);

        return _mapper.Map<SubTaskResponseDto>(subtask.SubTask);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var subtask = await _uow.SubTasks.Query()
            .Where(s => s.SubTaskId == id)
            .Join(_uow.Tasks.Query(), st => st.TaskId, t => t.MyTaskId, (st, t) => new { SubTask = st, Task = t })
            .FirstOrDefaultAsync(x => x.Task.UserId == userId)
            ?? throw new NotFoundException("SubTask", id);

        var taskId = subtask.SubTask.TaskId;

        _uow.SubTasks.Remove(subtask.SubTask);
        await _uow.SaveChangesAsync();

        await SyncTaskProgressAsync(taskId);
    }

    private async Task SyncTaskProgressAsync(int taskId)
    {
        var task = await _uow.Tasks.Query()
            .Include(t => t.SubTasks)
            .FirstOrDefaultAsync(t => t.MyTaskId == taskId);

        if (task is null) return;

        task.UpdateProgress();
        task.UpdatedAt = DateTime.UtcNow;
        _uow.Tasks.Update(task);
        await _uow.SaveChangesAsync();
    }
}
