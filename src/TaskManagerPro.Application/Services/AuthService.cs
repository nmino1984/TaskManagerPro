using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.Auth;
using TaskManagerPro.Application.Exceptions;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;

namespace TaskManagerPro.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUnitOfWork uow, IJwtTokenService jwtTokenService, IPasswordHasher passwordHasher)
    {
        _uow = uow;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _uow.Users.GetByUsernameAsync(dto.Username);

        if (existingUser != null)
        {
            throw new ValidationException($"Username '{dto.Username}' is already taken. Please choose a different username.");
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = _passwordHasher.Hash(dto.Password)
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        var token = _jwtTokenService.GenerateToken(user.UserId, user.Username);
        return new AuthResponseDto { Token = token, UserId = user.UserId, Username = user.Username };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _uow.Users.GetByUsernameAsync(dto.Username)
            ?? throw new ValidationException("Username or password is incorrect.");

        if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
            throw new ValidationException("Username or password is incorrect.");
        }

        var token = _jwtTokenService.GenerateToken(user.UserId, user.Username);
        return new AuthResponseDto { Token = token, UserId = user.UserId, Username = user.Username };
    }
}
