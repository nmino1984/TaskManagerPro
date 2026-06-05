using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.User;
namespace TaskManagerPro.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(string userId);
}


