using MyApp.Domain.Entities;

namespace MyApp.Application.Interfaces.Repositories;

public interface IMilestoneAuditLogRepository : IRepository<MilestoneAuditLog>
{
    Task<List<MilestoneAuditLog>> GetByMilestoneIdAsync(int milestoneId);
}
