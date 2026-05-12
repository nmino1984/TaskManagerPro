using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.Exceptions;
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

    public async Task<List<MyTaskResponseDto>> GetAllAsync(string userId)
    {
        var tasks = await _db.MyTasks
            .Where(t => t.UserId == userId)
            .Include(t => t.SubTasks)
            .Include(t => t.CalendarEvents)
            .ToListAsync();

        return _mapper.Map<List<MyTaskResponseDto>>(tasks);
    }

    public async Task<MyTaskResponseDto> GetByIdAsync(int id, string userId)
    {
        var task = await _db.MyTasks
            .Include(t => t.SubTasks)
            .Include(t => t.CalendarEvents)
            .FirstOrDefaultAsync(t => t.MyTaskId == id && t.UserId == userId)
            ?? throw new NotFoundException("MyTask", id);

        return _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task<MyTaskResponseDto> CreateAsync(MyTaskCreateDto dto, string userId)
    {
        var task = _mapper.Map<MyTask>(dto);
        task.UserId = userId;
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _db.MyTasks.Add(task);
        await _db.SaveChangesAsync();

        return _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task<MyTaskResponseDto> UpdateAsync(int id, MyTaskUpdateDto dto, string userId)
    {
        var task = await _db.MyTasks.FirstOrDefaultAsync(t => t.MyTaskId == id && t.UserId == userId)
            ?? throw new NotFoundException("MyTask", id);

        _mapper.Map(dto, task);
        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return _mapper.Map<MyTaskResponseDto>(task);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var task = await _db.MyTasks.FirstOrDefaultAsync(t => t.MyTaskId == id && t.UserId == userId)
            ?? throw new NotFoundException("MyTask", id);

        _db.MyTasks.Remove(task);
        await _db.SaveChangesAsync();
    }
}
