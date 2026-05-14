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

    public async Task<List<MyApp.Application.DTOs.Milestone.MilestoneResponseDto>> GetByTaskAsync(int taskId)
        => await _milestoneService.GetByTaskAsync(taskId);

    public async Task<MyApp.Application.DTOs.Milestone.MilestoneResponseDto> GetByIdAsync(int id)
        => await _milestoneService.GetByIdAsync(id);

    public async Task<MyApp.Application.DTOs.Milestone.MilestoneResponseDto> CreateAsync(MyApp.Application.DTOs.Milestone.MilestoneCreateDto dto)
        => await _milestoneService.CreateAsync(dto);

    public async Task<MyApp.Application.DTOs.Milestone.MilestoneResponseDto> UpdateAsync(int id, MyApp.Application.DTOs.Milestone.MilestoneUpdateDto dto)
        => await _milestoneService.UpdateAsync(id, dto);

    public async Task DeleteAsync(int id)
        => await _milestoneService.DeleteAsync(id);
}
