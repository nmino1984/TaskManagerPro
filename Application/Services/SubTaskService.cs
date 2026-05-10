using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.SubTask;
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

    public async Task<List<SubTaskResponseDto>> GetByTaskAsync(int taskId)
    {
        var subtasks = await _db.SubTasks
            .Where(s => s.TaskId == taskId)
            .ToListAsync();

        return _mapper.Map<List<SubTaskResponseDto>>(subtasks);
    }

    public async Task<SubTaskResponseDto?> GetByIdAsync(int id)
    {
        var subtask = await _db.SubTasks.FindAsync(id);
        return subtask is null ? null : _mapper.Map<SubTaskResponseDto>(subtask);
    }

    public async Task<SubTaskResponseDto?> CreateAsync(SubTaskCreateDto dto)
    {
        var taskExists = await _db.MyTasks.AnyAsync(t => t.MyTaskId == dto.TaskId);
        if (!taskExists) return null;

        var subtask = _mapper.Map<SubTask>(dto);
        subtask.CreatedAt = DateTime.UtcNow;
        subtask.UpdatedAt = DateTime.UtcNow;

        _db.SubTasks.Add(subtask);
        await _db.SaveChangesAsync();

        await SyncTaskProgressAsync(dto.TaskId);

        return _mapper.Map<SubTaskResponseDto>(subtask);
    }

    public async Task<SubTaskResponseDto?> UpdateAsync(int id, SubTaskUpdateDto dto)
    {
        var subtask = await _db.SubTasks.FindAsync(id);
        if (subtask is null) return null;

        _mapper.Map(dto, subtask);
        subtask.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await SyncTaskProgressAsync(subtask.TaskId);

        return _mapper.Map<SubTaskResponseDto>(subtask);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subtask = await _db.SubTasks.FindAsync(id);
        if (subtask is null) return false;

        var taskId = subtask.TaskId;

        _db.SubTasks.Remove(subtask);
        await _db.SaveChangesAsync();

        await SyncTaskProgressAsync(taskId);

        return true;
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
