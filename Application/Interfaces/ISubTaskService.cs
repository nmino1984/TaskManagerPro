using MyApp.Application.DTOs.SubTask;

namespace MyApp.Application.Interfaces;

public interface ISubTaskService
{
    Task<List<SubTaskResponseDto>> GetByTaskAsync(int taskId);
    Task<SubTaskResponseDto?> GetByIdAsync(int id);
    Task<SubTaskResponseDto?> CreateAsync(SubTaskCreateDto dto);
    Task<SubTaskResponseDto?> UpdateAsync(int id, SubTaskUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
