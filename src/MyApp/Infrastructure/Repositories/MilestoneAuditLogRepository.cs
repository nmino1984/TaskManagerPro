using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories;

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
