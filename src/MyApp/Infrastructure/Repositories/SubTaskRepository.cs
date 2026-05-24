using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories;

public class SubTaskRepository : Repository<SubTask>, ISubTaskRepository
{
    public SubTaskRepository(AppDbContext context) : base(context)
    {
    }
}
