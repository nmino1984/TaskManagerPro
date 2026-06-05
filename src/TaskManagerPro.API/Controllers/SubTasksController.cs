using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerPro.Application.DTOs.SubTask;
using TaskManagerPro.Application.Interfaces;

namespace TaskManagerPro.API.Controllers;

/// <summary>
/// Manages subtasks belonging to a parent task.
/// </summary>
[ApiController]
[Route("api/v1/subtasks")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class SubTasksController : ControllerBase
{
    private readonly ISubTaskService _subTaskService;
    private readonly IAuditLogService _auditLogService;

    public SubTasksController(ISubTaskService subTaskService, IAuditLogService auditLogService)
    {
        _subTaskService = subTaskService;
        _auditLogService = auditLogService;
    }

    [HttpGet("bytask/{taskId:int}")]
    [ProducesResponseType<List<SubTaskResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _subTaskService.GetByTaskAsync(taskId, userId));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<SubTaskResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _subTaskService.GetByIdAsync(id, userId));
    }

    [HttpPost]
    [ProducesResponseType<SubTaskResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(SubTaskCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        var result = await _subTaskService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.SubTaskId }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<SubTaskResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, SubTaskUpdateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _subTaskService.UpdateAsync(id, dto, userId));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        await _subTaskService.DeleteAsync(id, userId);
        return NoContent();
    }

    /// <summary>
    /// Retrieves the audit history for a specific subtask.
    /// </summary>
    [HttpGet("{id:int}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _auditLogService.GetSubTaskHistoryAsync(id, userId));
    }
}
