using System;
using TaskManagerPro.Application.DTOs.Common;
using TaskManagerPro.Application.DTOs.MyTask;

using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces;

public interface ITaskService
{
    Task<PagedResult<MyTaskResponseDto>> GetAllAsync(string userId, TaskQueryParams q);
    Task<MyTaskResponseDto> GetByIdAsync(int id, string userId);
    Task<MyTaskResponseDto> CreateAsync(MyTaskCreateDto dto, string userId);
    Task<MyTaskResponseDto> UpdateAsync(int id, MyTaskUpdateDto dto, string userId);
    Task DeleteAsync(int id, string userId);
    Task<TaskAssignmentResponseDto> AssignAsync(int id, AssignTaskDto dto, string userId);
}

