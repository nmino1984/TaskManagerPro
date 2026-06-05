using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;
using TaskManagerPro.Infrastructure.Persistence;

namespace TaskManagerPro.Infrastructure.Repositories;

public class MilestoneRepository : IMilestoneRepository
{
    private readonly AppDbContext _context;

    public MilestoneRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Milestone>> GetByTaskIdAsync(int taskId)
    {
        return await _context.Milestones
            .Where(m => m.TaskId == taskId)
            .ToListAsync();
    }

    public async Task<Milestone?> GetByIdAndUserIdAsync(int id, string userId)
    {
        return await _context.Milestones
            .Include(m => m.Task)
            .FirstOrDefaultAsync(m => m.MilestoneId == id && m.Task.UserId == userId);
    }

    public async Task AddAsync(Milestone entity)
    {
        await _context.Milestones.AddAsync(entity);
    }

    public void Update(Milestone entity)
    {
        _context.Milestones.Update(entity);
    }

    public void Remove(Milestone entity)
    {
        _context.Milestones.Remove(entity);
    }

    public async Task<Milestone?> GetByIdAsync(object id)
    {
        return await _context.Milestones.FirstOrDefaultAsync(m => m.MilestoneId == (int)id);
    }

    public IQueryable<Milestone> Query()
    {
        return _context.Milestones;
    }
}
