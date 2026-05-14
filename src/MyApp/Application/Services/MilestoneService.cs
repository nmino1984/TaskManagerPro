using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.Milestone;
using MyApp.Application.Exceptions;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Application.Services;

public class MilestoneService : IMilestoneService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public MilestoneService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<MilestoneResponseDto>> GetByTaskAsync(int taskId)
    {
        var milestones = await _db.Milestones
            .Where(m => m.TaskId == taskId)
            .ToListAsync();

        return _mapper.Map<List<MilestoneResponseDto>>(milestones);
    }

    public async Task<MilestoneResponseDto> GetByIdAsync(int id)
    {
        var milestone = await _db.Milestones.FindAsync(id)
            ?? throw new NotFoundException("Milestone", id);

        return _mapper.Map<MilestoneResponseDto>(milestone);
    }

    public async Task<MilestoneResponseDto> CreateAsync(MilestoneCreateDto dto)
    {
        var taskExists = await _db.MyTasks.AnyAsync(t => t.MyTaskId == dto.TaskId);
        if (!taskExists)
            throw new NotFoundException("MyTask", dto.TaskId);

        var milestone = _mapper.Map<Milestone>(dto);
        milestone.CreatedAt = DateTime.UtcNow;
        milestone.UpdatedAt = DateTime.UtcNow;

        _db.Milestones.Add(milestone);
        await _db.SaveChangesAsync();

        return _mapper.Map<MilestoneResponseDto>(milestone);
    }

    public async Task<MilestoneResponseDto> UpdateAsync(int id, MilestoneUpdateDto dto)
    {
        var milestone = await _db.Milestones.FindAsync(id)
            ?? throw new NotFoundException("Milestone", id);

        _mapper.Map(dto, milestone);
        milestone.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return _mapper.Map<MilestoneResponseDto>(milestone);
    }

    public async Task DeleteAsync(int id)
    {
        var milestone = await _db.Milestones.FindAsync(id)
            ?? throw new NotFoundException("Milestone", id);

        _db.Milestones.Remove(milestone);
        await _db.SaveChangesAsync();
    }
}
