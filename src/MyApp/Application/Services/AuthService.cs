using Microsoft.IdentityModel.Tokens;
using MyApp.Application.DTOs.Auth;
using MyApp.Application.Exceptions;
using MyApp.Application.Interfaces;
using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _config;

    public AuthService(IUnitOfWork uow, IConfiguration config)
    {
        _uow = uow;
        _config = config;
    }

    private string GetJwtKey()
    {
        var key = Environment.GetEnvironmentVariable("JWT_KEY");
        if (string.IsNullOrEmpty(key))
        {
            key = "development-key-that-must-be-at-least-32-characters-long!!!";
        }
        return key;
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        return new AuthResponseDto { Token = token, UserId = user.UserId, Username = user.Username };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _uow.Users.GetByUsernameAsync(dto.Username)
            ?? throw new ValidationException($"Username or password is incorrect.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new ValidationException("Username or password is incorrect.");
        }

        var token = GenerateJwtToken(user);
        return new AuthResponseDto { Token = token, UserId = user.UserId, Username = user.Username };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtKey()));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiresInMinutes"] ?? "60")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
