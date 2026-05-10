using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Application.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public TaskService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<MyTaskResponseDto>> GetAllAsync()
    {
        var tasks = await _db.MyTasks
            .Include(t => t.SubTasks)
            .Include(t => t.CalendarEvents)
            .ToListAsync();

        return _mapper.Map<List<MyTaskResponseDto>>(tasks);
    }

    public async Task<MyTaskResponseDto?> GetByIdAsync(int id)
    {
        var task = await _db.MyTasks
            .Include(t => t.SubTasks)
            .Include(t => t.CalendarEvents)
            .FirstOrDefaultAsync(t => t.MyTaskId == id);

        return task is null ? null : _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task<MyTaskResponseDto> CreateAsync(MyTaskCreateDto dto)
    {
        var task = _mapper.Map<MyTask>(dto);
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _db.MyTasks.Add(task);
        await _db.SaveChangesAsync();

        return _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task<MyTaskResponseDto?> UpdateAsync(int id, MyTaskUpdateDto dto)
    {
        var task = await _db.MyTasks.FindAsync(id);
        if (task is null) return null;

        _mapper.Map(dto, task);
        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _db.MyTasks.FindAsync(id);
        if (task is null) return false;

        _db.MyTasks.Remove(task);
        await _db.SaveChangesAsync();

        return true;
    }
}
