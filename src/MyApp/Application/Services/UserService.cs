using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.User;
using MyApp.Application.Interfaces;
using MyApp.Application.Interfaces.Repositories;

namespace MyApp.Application.Services;

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
        var users = await _uow.Users.Query().ToListAsync();
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(string userId)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }
}
