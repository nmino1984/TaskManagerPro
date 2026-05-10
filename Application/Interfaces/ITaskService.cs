using MyApp.Application.DTOs.MyTask;

namespace MyApp.Application.Interfaces;

public interface ITaskService
{
    Task<List<MyTaskResponseDto>> GetAllAsync();
    Task<MyTaskResponseDto?> GetByIdAsync(int id);
    Task<MyTaskResponseDto> CreateAsync(MyTaskCreateDto dto);
    Task<MyTaskResponseDto?> UpdateAsync(int id, MyTaskUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
