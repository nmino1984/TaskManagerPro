using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Application.DTOs.Common;
using TaskManagerPro.Application.DTOs.MyTask;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;
using TaskManagerPro.Infrastructure.Persistence;

namespace TaskManagerPro.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<MyTask>> GetPagedAsync(string userId, TaskQueryParams q)
    {
        var query = _context.MyTasks
            .Where(t => t.UserId == userId)
            .AsQueryable();

        if (q.Status.HasValue)
            query = query.Where(t => t.Status == q.Status);

        if (q.Priority.HasValue)
            query = query.Where(t => t.Priority == q.Priority);

        if (!string.IsNullOrWhiteSpace(q.Search))
            query = query.Where(t => t.Title.Contains(q.Search) || (t.Description != null && t.Description.Contains(q.Search)));

        var totalCount = await query.CountAsync();

        var validPageSize = Math.Min(q.PageSize, 100);
        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((q.Page - 1) * validPageSize)
            .Take(validPageSize)
            .Include(t => t.SubTasks)
            .Include(t => t.Milestones)
            .Include(t => t.AssignedToUser)
            .ToListAsync();

        return new PagedResult<MyTask>
        {
            Items = tasks,
            TotalCount = totalCount,
            Page = q.Page,
            PageSize = validPageSize
        };
    }

    public async Task<MyTask?> GetByIdWithIncludesAsync(int id, string userId)
    {
        return await _context.MyTasks
            .Include(t => t.SubTasks)
            .Include(t => t.Milestones)
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t => t.MyTaskId == id && t.UserId == userId);
    }

    public async Task<bool> ExistsForUserAsync(int taskId, string userId)
    {
        return await _context.MyTasks
            .AnyAsync(t => t.MyTaskId == taskId && t.UserId == userId);
    }

    public async Task<MyTask?> GetByIdWithSubTasksAsync(int taskId)
    {
        return await _context.MyTasks
            .Include(t => t.SubTasks)
            .FirstOrDefaultAsync(t => t.MyTaskId == taskId);
    }

    public async Task AddAsync(MyTask entity)
    {
        await _context.MyTasks.AddAsync(entity);
    }

    public void Update(MyTask entity)
    {
        _context.MyTasks.Update(entity);
    }

    public void Remove(MyTask entity)
    {
        _context.MyTasks.Remove(entity);
    }

    public async Task<MyTask?> GetByIdAsync(object id)
    {
        return await _context.MyTasks.FirstOrDefaultAsync(t => t.MyTaskId == (int)id);
    }

    public IQueryable<MyTask> Query()
    {
        return _context.MyTasks;
    }
}
