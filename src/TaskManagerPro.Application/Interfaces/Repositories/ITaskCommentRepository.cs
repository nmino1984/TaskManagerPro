using System;
using TaskManagerPro.Domain.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface ITaskCommentRepository : IRepository<TaskComment>
{
    Task<List<TaskComment>> GetByTaskIdAsync(int taskId);
}


