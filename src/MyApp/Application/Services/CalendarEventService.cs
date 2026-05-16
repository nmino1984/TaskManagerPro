// DEPRECATED: Use MilestoneService instead
// This file is kept for backwards compatibility but redirects to MilestoneService

using AutoMapper;
using MyApp.Application.Interfaces;
using MyApp.Infrastructure.Data;

namespace MyApp.Application.Services;

public class CalendarEventService : IMilestoneService
{
    private readonly MilestoneService _milestoneService;

    public CalendarEventService(AppDbContext db, IMapper mapper)
    {
        _milestoneService = new MilestoneService(db, mapper);
    }

    public async Task<List<MyApp.Application.DTOs.Milestone.MilestoneResponseDto>> GetByTaskAsync(int taskId, string userId)
        => await _milestoneService.GetByTaskAsync(taskId, userId);

    public async Task<MyApp.Application.DTOs.Milestone.MilestoneResponseDto> GetByIdAsync(int id, string userId)
        => await _milestoneService.GetByIdAsync(id, userId);

    public async Task<MyApp.Application.DTOs.Milestone.MilestoneResponseDto> CreateAsync(MyApp.Application.DTOs.Milestone.MilestoneCreateDto dto, string userId)
        => await _milestoneService.CreateAsync(dto, userId);

    public async Task<MyApp.Application.DTOs.Milestone.MilestoneResponseDto> UpdateAsync(int id, MyApp.Application.DTOs.Milestone.MilestoneUpdateDto dto, string userId)
        => await _milestoneService.UpdateAsync(id, dto, userId);

    public async Task DeleteAsync(int id, string userId)
        => await _milestoneService.DeleteAsync(id, userId);

    public async Task<byte[]> ExportToJsonAsync(int taskId, string userId)
        => await _milestoneService.ExportToJsonAsync(taskId, userId);

    public async Task<byte[]> ExportToXmlAsync(int taskId, string userId)
        => await _milestoneService.ExportToXmlAsync(taskId, userId);

    public async Task<byte[]> ExportToICalAsync(int taskId, string userId)
        => await _milestoneService.ExportToICalAsync(taskId, userId);
}
