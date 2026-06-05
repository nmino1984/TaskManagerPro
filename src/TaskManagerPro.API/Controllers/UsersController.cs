using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerPro.Application.Interfaces;

namespace TaskManagerPro.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(new { items = users });
    }
}
