using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.SubTask;
namespace TaskManagerPro.Application.Interfaces;

public interface ISubTaskService
{
    Task<List<SubTaskResponseDto>> GetByTaskAsync(int taskId, string userId);
    Task<SubTaskResponseDto> GetByIdAsync(int id, string userId);
    Task<SubTaskResponseDto> CreateAsync(SubTaskCreateDto dto, string userId);
    Task<SubTaskResponseDto> UpdateAsync(int id, SubTaskUpdateDto dto, string userId);
    Task DeleteAsync(int id, string userId);
}


