using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories;

public class MilestoneRepository : Repository<Milestone>, IMilestoneRepository
{
    public MilestoneRepository(AppDbContext context) : base(context)
    {
    }
}
