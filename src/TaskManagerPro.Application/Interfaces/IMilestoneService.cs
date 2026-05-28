using System;
using TaskManagerPro.Application.DTOs.Milestone;

using System.Collections.Generic;
using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces;

public interface IMilestoneService
{
    Task<List<MilestoneResponseDto>> GetByTaskAsync(int taskId, string userId);
    Task<MilestoneResponseDto> GetByIdAsync(int id, string userId);
    Task<MilestoneResponseDto> CreateAsync(MilestoneCreateDto dto, string userId);
    Task<MilestoneResponseDto> UpdateAsync(int id, MilestoneUpdateDto dto, string userId);
    Task DeleteAsync(int id, string userId);
    Task<byte[]> ExportToJsonAsync(int taskId, string userId);
    Task<byte[]> ExportToXmlAsync(int taskId, string userId);
    Task<byte[]> ExportToICalAsync(int taskId, string userId);
}


