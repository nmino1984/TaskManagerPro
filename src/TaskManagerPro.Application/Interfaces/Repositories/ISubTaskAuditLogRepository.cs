using System;
using TaskManagerPro.Domain.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface ISubTaskAuditLogRepository : IRepository<SubTaskAuditLog>
{
    Task<List<SubTaskAuditLog>> GetBySubTaskIdAsync(int subTaskId);
}


