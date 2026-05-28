using System;
using System.Collections.Generic;
using TaskManagerPro.Domain.Entities;

using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(string userId);
    Task<List<User>> GetAllAsync();
}

