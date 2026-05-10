using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.SubTask;
using MyApp.Application.Interfaces;

namespace MyApp.Api.Controllers;

/// <summary>
/// Manages subtasks belonging to a parent task.
/// </summary>
[ApiController]
[Route("api/v1/subtasks")]
[Produces("application/json")]
[Consumes("application/json")]
public class SubTasksController : ControllerBase
{
    private readonly ISubTaskService _subTaskService;

    public SubTasksController(ISubTaskService subTaskService)
    {
        _subTaskService = subTaskService;
    }

    /// <summary>
    /// Retrieves all subtasks belonging to a specific task.
    /// </summary>
    /// <param name="taskId">The parent task ID.</param>
    [HttpGet("bytask/{taskId:int}")]
    [ProducesResponseType<List<SubTaskResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTask(int taskId)
        => Ok(await _subTaskService.GetByTaskAsync(taskId));

    /// <summary>
    /// Retrieves a single subtask by its ID.
    /// </summary>
    /// <param name="id">The subtask ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType<SubTaskResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _subTaskService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new subtask under an existing task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<SubTaskResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(SubTaskCreateDto dto)
    {
        var result = await _subTaskService.CreateAsync(dto);
        if (result is null)
            return BadRequest("The parent task does not exist.");

        return CreatedAtAction(nameof(GetById), new { id = result.SubTaskId }, result);
    }

    /// <summary>
    /// Updates an existing subtask.
    /// </summary>
    /// <param name="id">The subtask ID.</param>
    /// <param name="dto">The updated subtask data.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType<SubTaskResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, SubTaskUpdateDto dto)
    {
        var result = await _subTaskService.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Deletes a subtask by its ID.
    /// </summary>
    /// <param name="id">The subtask ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
        => await _subTaskService.DeleteAsync(id) ? NoContent() : NotFound();
}
