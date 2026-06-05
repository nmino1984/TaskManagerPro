using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.Auth;
namespace TaskManagerPro.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}

