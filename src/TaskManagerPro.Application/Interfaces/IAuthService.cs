using System;
using TaskManagerPro.Application.DTOs.Auth;

using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}

