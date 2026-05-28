using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Domain.Entities;

namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface IMilestoneRepository : IRepository<Milestone>
{
    Task<List<Milestone>> GetByTaskIdAsync(int taskId);
    Task<Milestone?> GetByIdAndUserIdAsync(int id, string userId);
}
