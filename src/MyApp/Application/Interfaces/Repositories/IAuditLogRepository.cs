using MyApp.Domain.Entities;

namespace MyApp.Application.Interfaces.Repositories;

public interface IAuditLogRepository : IRepository<TaskAuditLog>
{
    Task<List<TaskAuditLog>> GetByTaskIdAsync(int taskId, string userId);
}
