using MyApp.Application.DTOs.MyTask;

namespace MyApp.Application.Interfaces;

public interface ITaskService
{
    Task<List<MyTaskResponseDto>> GetAllAsync(string userId);
    Task<MyTaskResponseDto> GetByIdAsync(int id, string userId);
    Task<MyTaskResponseDto> CreateAsync(MyTaskCreateDto dto, string userId);
    Task<MyTaskResponseDto> UpdateAsync(int id, MyTaskUpdateDto dto, string userId);
    Task DeleteAsync(int id, string userId);
}
