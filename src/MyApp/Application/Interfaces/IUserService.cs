using MyApp.Application.DTOs.User;

namespace MyApp.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(string userId);
}
