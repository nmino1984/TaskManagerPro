using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories;

public class AuditLogRepository : Repository<TaskAuditLog>, IAuditLogRepository
{
    public AuditLogRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<TaskAuditLog>> GetByTaskIdAsync(int taskId, string userId)
    {
        return await _context.TaskAuditLogs
            .Where(a => a.TaskId == taskId && a.Task.UserId == userId)
            .Include(a => a.ChangedByUser)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync();
    }
}
