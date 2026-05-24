using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories;

public class SubTaskAuditLogRepository : Repository<SubTaskAuditLog>, ISubTaskAuditLogRepository
{
    public SubTaskAuditLogRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<SubTaskAuditLog>> GetBySubTaskIdAsync(int subTaskId)
    {
        return await _context.SubTaskAuditLogs
            .Where(a => a.SubTaskId == subTaskId)
            .Include(a => a.ChangedByUser)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync();
    }
}
