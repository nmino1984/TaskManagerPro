using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.Interfaces;

namespace MyApp.Api.Controllers;

/// <summary>
/// Manages tasks including their subtasks and calendar events.
/// </summary>
[ApiController]
[Route("api/v1/tasks")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class MyTaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public MyTaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Retrieves all tasks including their subtasks and calendar events.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<List<MyTaskResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MyTaskResponseDto>>> GetAll()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _taskService.GetAllAsync(userId));
    }

    /// <summary>
    /// Retrieves a single task by its ID.
    /// </summary>
    /// <param name="id">The task ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType<MyTaskResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MyTaskResponseDto>> GetById(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _taskService.GetByIdAsync(id, userId));
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<MyTaskResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MyTaskResponseDto>> Create(MyTaskCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        var result = await _taskService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.MyTaskId }, result);
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="dto">The updated task data.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType<MyTaskResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MyTaskResponseDto>> Update(int id, MyTaskUpdateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _taskService.UpdateAsync(id, dto, userId));
    }

    /// <summary>
    /// Deletes a task by its ID.
    /// </summary>
    /// <param name="id">The task ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        await _taskService.DeleteAsync(id, userId);
        return NoContent();
    }
}
