using MyApp.Application.DTOs.Common;
using MyApp.Application.DTOs.MyTask;
using MyApp.Domain.Entities;

namespace MyApp.Application.Interfaces.Repositories;

public interface ITaskRepository : IRepository<MyTask>
{
    Task<PagedResult<MyTask>> GetPagedAsync(string userId, TaskQueryParams q);
    Task<MyTask?> GetByIdWithIncludesAsync(int id, string userId);
}
