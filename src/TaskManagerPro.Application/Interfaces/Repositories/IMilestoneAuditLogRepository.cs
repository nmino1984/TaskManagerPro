using System;
using TaskManagerPro.Domain.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface IMilestoneAuditLogRepository : IRepository<MilestoneAuditLog>
{
    Task<List<MilestoneAuditLog>> GetByMilestoneIdAsync(int milestoneId);
}


