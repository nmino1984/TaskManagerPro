using System;
using TaskManagerPro.Domain.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface IAuditLogRepository : IRepository<TaskAuditLog>
{
    Task<List<TaskAuditLog>> GetByTaskIdAsync(int taskId, string userId);
}


