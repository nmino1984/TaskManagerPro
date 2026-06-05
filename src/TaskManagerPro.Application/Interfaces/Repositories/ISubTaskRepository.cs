using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Domain.Entities;

namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface ISubTaskRepository : IRepository<SubTask>
{
    Task<List<SubTask>> GetByTaskIdAsync(int taskId);
    Task<SubTask?> GetByIdAndUserIdAsync(int id, string userId);
}
