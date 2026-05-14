using MyApp.Application.DTOs.Milestone;

namespace MyApp.Application.Interfaces;

public interface IMilestoneService
{
    Task<List<MilestoneResponseDto>> GetByTaskAsync(int taskId);
    Task<MilestoneResponseDto> GetByIdAsync(int id);
    Task<MilestoneResponseDto> CreateAsync(MilestoneCreateDto dto);
    Task<MilestoneResponseDto> UpdateAsync(int id, MilestoneUpdateDto dto);
    Task DeleteAsync(int id);
}
