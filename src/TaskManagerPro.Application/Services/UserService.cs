using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.User;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Application.Interfaces.Repositories;
namespace TaskManagerPro.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _uow.Users.GetAllAsync();
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(string userId)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }
}



