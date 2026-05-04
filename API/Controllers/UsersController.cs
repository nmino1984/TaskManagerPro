using Microsoft.AspNetCore.Mvc;

namespace MyApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Users endpoint funcionando");
    }
}