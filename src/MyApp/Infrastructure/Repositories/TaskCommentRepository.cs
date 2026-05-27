using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories;

public class TaskCommentRepository : Repository<TaskComment>, ITaskCommentRepository
{
    public TaskCommentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<TaskComment>> GetByTaskIdAsync(int taskId)
    {
        return await _context.TaskComments
            .Where(c => c.TaskId == taskId && c.ParentCommentId == null)
            .Include(c => c.CreatedByUser)
            .Include(c => c.Replies)
            .ThenInclude(r => r.CreatedByUser)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }
}
