using System;
using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.Common;
using TaskManagerPro.Application.DTOs.MyTask;
using TaskManagerPro.Domain.Entities;

namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface ITaskRepository : IRepository<MyTask>
{
    Task<PagedResult<MyTask>> GetPagedAsync(string userId, TaskQueryParams q);
    Task<MyTask?> GetByIdWithIncludesAsync(int id, string userId);
    Task<bool> ExistsForUserAsync(int taskId, string userId);
    Task<MyTask?> GetByIdWithSubTasksAsync(int taskId);
}

