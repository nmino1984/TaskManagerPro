using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Domain.Entities;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface ISubTaskAuditLogRepository : IRepository<SubTaskAuditLog>
{
    Task<List<SubTaskAuditLog>> GetBySubTaskIdAsync(int subTaskId);
}


