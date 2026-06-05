using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Domain.Entities;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface IMilestoneAuditLogRepository : IRepository<MilestoneAuditLog>
{
    Task<List<MilestoneAuditLog>> GetByMilestoneIdAsync(int milestoneId);
}


