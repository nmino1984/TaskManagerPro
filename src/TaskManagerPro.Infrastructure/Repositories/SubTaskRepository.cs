using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;
using TaskManagerPro.Infrastructure.Persistence;

namespace TaskManagerPro.Infrastructure.Repositories;

public class SubTaskRepository : ISubTaskRepository
{
    private readonly AppDbContext _context;

    public SubTaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubTask>> GetByTaskIdAsync(int taskId)
    {
        return await _context.SubTasks
            .Where(s => s.TaskId == taskId)
            .ToListAsync();
    }

    public async Task<SubTask?> GetByIdAndUserIdAsync(int id, string userId)
    {
        return await _context.SubTasks
            .Where(s => s.SubTaskId == id)
            .Join(_context.MyTasks,
                  s => s.TaskId,
                  t => t.MyTaskId,
                  (s, t) => new { SubTask = s, Task = t })
            .Where(x => x.Task.UserId == userId)
            .Select(x => x.SubTask)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(SubTask entity)
    {
        await _context.SubTasks.AddAsync(entity);
    }

    public void Update(SubTask entity)
    {
        _context.SubTasks.Update(entity);
    }

    public void Remove(SubTask entity)
    {
        _context.SubTasks.Remove(entity);
    }

    public async Task<SubTask?> GetByIdAsync(object id)
    {
        return await _context.SubTasks.FirstOrDefaultAsync(s => s.SubTaskId == (int)id);
    }

    public IQueryable<SubTask> Query()
    {
        return _context.SubTasks;
    }
}
