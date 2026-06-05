using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Domain.Entities;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface ITaskCommentRepository : IRepository<TaskComment>
{
    Task<List<TaskComment>> GetByTaskIdAsync(int taskId);
}


