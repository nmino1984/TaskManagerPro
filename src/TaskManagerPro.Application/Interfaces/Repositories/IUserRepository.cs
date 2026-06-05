using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Domain.Entities;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(string userId);
    Task<List<User>> GetAllAsync();
}

