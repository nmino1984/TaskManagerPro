using MyApp.Domain.Entities;

namespace MyApp.Application.Interfaces.Repositories;

public interface ITaskCommentRepository : IRepository<TaskComment>
{
    Task<List<TaskComment>> GetByTaskIdAsync(int taskId);
}
