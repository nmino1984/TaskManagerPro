using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Domain.Entities;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface IAuditLogRepository : IRepository<TaskAuditLog>
{
    Task<List<TaskAuditLog>> GetByTaskIdAsync(int taskId, string userId);
}


