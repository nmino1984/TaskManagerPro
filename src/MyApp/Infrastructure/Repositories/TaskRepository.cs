using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.Common;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories;

public class TaskRepository : Repository<MyTask>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context)
    {
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
}
