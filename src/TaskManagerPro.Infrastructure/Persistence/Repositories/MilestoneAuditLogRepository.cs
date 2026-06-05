using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;

namespace TaskManagerPro.Infrastructure.Persistence.Repositories;

public class MilestoneAuditLogRepository : Repository<MilestoneAuditLog>, IMilestoneAuditLogRepository
{
    public MilestoneAuditLogRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<MilestoneAuditLog>> GetByMilestoneIdAsync(int milestoneId)
    {
        return await _context.MilestoneAuditLogs
            .Where(a => a.MilestoneId == milestoneId)
            .Include(a => a.ChangedByUser)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync();
    }
}
