using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.SubTask;
using MyApp.Application.Exceptions;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Application.Services;

public class SubTaskService : ISubTaskService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public SubTaskService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<SubTaskResponseDto>> GetByTaskAsync(int taskId, string userId)
    {
        var taskExists = await _db.MyTasks
            .AnyAsync(t => t.MyTaskId == taskId && t.UserId == userId);

        if (!taskExists)
            throw new NotFoundException("MyTask", taskId);

        var subtasks = await _db.SubTasks
            .Where(s => s.TaskId == taskId)
            .ToListAsync();

        return _mapper.Map<List<SubTaskResponseDto>>(subtasks);
    }

    public async Task<SubTaskResponseDto> GetByIdAsync(int id, string userId)
    {
        var subtask = await _db.SubTasks
            .Where(s => s.SubTaskId == id)
            .Join(_db.MyTasks, st => st.TaskId, t => t.MyTaskId, (st, t) => new { SubTask = st, Task = t })
            .FirstOrDefaultAsync(x => x.Task.UserId == userId)
            ?? throw new NotFoundException("SubTask", id);

        return _mapper.Map<SubTaskResponseDto>(subtask.SubTask);
    }

    public async Task<SubTaskResponseDto> CreateAsync(SubTaskCreateDto dto, string userId)
    {
        var task = await _db.MyTasks.FirstOrDefaultAsync(t => t.MyTaskId == dto.TaskId)
            ?? throw new NotFoundException("MyTask", dto.TaskId);

        if (task.UserId != userId)
            throw new UnauthorizedAccessException("You do not have access to this task.");

        var subtask = _mapper.Map<SubTask>(dto);
        subtask.CreatedAt = DateTime.UtcNow;
        subtask.UpdatedAt = DateTime.UtcNow;

        _db.SubTasks.Add(subtask);
        await _db.SaveChangesAsync();

        await SyncTaskProgressAsync(dto.TaskId);

        return _mapper.Map<SubTaskResponseDto>(subtask);
    }

    public async Task<SubTaskResponseDto> UpdateAsync(int id, SubTaskUpdateDto dto, string userId)
    {
        var subtask = await _db.SubTasks
            .Where(s => s.SubTaskId == id)
            .Join(_db.MyTasks, st => st.TaskId, t => t.MyTaskId, (st, t) => new { SubTask = st, Task = t })
            .FirstOrDefaultAsync(x => x.Task.UserId == userId)
            ?? throw new NotFoundException("SubTask", id);

        _mapper.Map(dto, subtask.SubTask);
        subtask.SubTask.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await SyncTaskProgressAsync(subtask.SubTask.TaskId);

        return _mapper.Map<SubTaskResponseDto>(subtask.SubTask);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var subtask = await _db.SubTasks
            .Where(s => s.SubTaskId == id)
            .Join(_db.MyTasks, st => st.TaskId, t => t.MyTaskId, (st, t) => new { SubTask = st, Task = t })
            .FirstOrDefaultAsync(x => x.Task.UserId == userId)
            ?? throw new NotFoundException("SubTask", id);

        var taskId = subtask.SubTask.TaskId;

        _db.SubTasks.Remove(subtask.SubTask);
        await _db.SaveChangesAsync();

        await SyncTaskProgressAsync(taskId);
    }

    private async Task SyncTaskProgressAsync(int taskId)
    {
        var task = await _db.MyTasks
            .Include(t => t.SubTasks)
            .FirstOrDefaultAsync(t => t.MyTaskId == taskId);

        if (task is null) return;

        task.UpdateProgress();
        task.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
