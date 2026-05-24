using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
    /// Retrieves all tasks with optional pagination and filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] TaskQueryParams q)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _taskService.GetAllAsync(userId, q));
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

    /// <summary>
    /// Assigns a task to another user.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="dto">Assignment data (AssignToUserId or null to unassign).</param>
    [HttpPatch("{id:int}/assign")]
    [ProducesResponseType<TaskAssignmentResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskAssignmentResponseDto>> Assign(int id, AssignTaskDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _taskService.AssignAsync(id, dto, userId));
    }

    /// <summary>
    /// DEBUG: Check database schema
    /// </summary>
    [HttpGet("debug/schema")]
    [AllowAnonymous]
    public IActionResult GetDbSchema()
    {
        var properties = typeof(MyTask).GetProperties();
        var props = properties.Select(p => new { PropertyName = p.Name, PropertyType = p.PropertyType.Name }).ToList();
        return Ok(new { message = "MyTask properties", properties = props });
    }

    /// <summary>
    /// DEBUG: Check actual database by querying first task
    /// </summary>
    [HttpGet("debug/db-columns")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDbColumns([FromServices] AppDbContext db)
    {
        try
        {
            var task = await db.MyTasks.FirstOrDefaultAsync();
            if (task == null)
                return Ok(new { message = "No tasks found in database" });

            return Ok(new {
                message = "First task from database",
                myTaskId = task.MyTaskId,
                title = task.Title,
                assignedToUserId = task.AssignedToUserId,
                assignedToUser = task.AssignedToUser?.Username
            });
        }
        catch (Exception ex)
        {
            return Ok(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}
