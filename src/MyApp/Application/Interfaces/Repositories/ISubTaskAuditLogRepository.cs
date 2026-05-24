using MyApp.Domain.Entities;

namespace MyApp.Application.Interfaces.Repositories;

public interface ISubTaskAuditLogRepository : IRepository<SubTaskAuditLog>
{
    Task<List<SubTaskAuditLog>> GetBySubTaskIdAsync(int subTaskId);
}
