using System;
using TaskManagerPro.Application.DTOs.User;

using System.Collections.Generic;
using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(string userId);
}


