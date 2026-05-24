using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.Milestone;
using MyApp.Application.Interfaces;

namespace MyApp.Api.Controllers;

/// <summary>
/// Manages milestones (important objectives) belonging to a parent task.
/// Milestones represent key checkpoints like code reviews, deliveries, or validations.
/// </summary>
[ApiController]
[Route("api/v1/milestones")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class MilestonesController : ControllerBase
{
    private readonly IMilestoneService _milestoneService;
    private readonly IAuditLogService _auditLogService;

    public MilestonesController(IMilestoneService milestoneService, IAuditLogService auditLogService)
    {
        _milestoneService = milestoneService;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Retrieves all milestones belonging to a specific task.
    /// </summary>
    /// <param name="taskId">The parent task ID.</param>
    [HttpGet("bytask/{taskId:int}")]
    [ProducesResponseType<List<MilestoneResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _milestoneService.GetByTaskAsync(taskId, userId));
    }

    /// <summary>
    /// Retrieves a single milestone by its ID.
    /// </summary>
    /// <param name="id">The milestone ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType<MilestoneResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _milestoneService.GetByIdAsync(id, userId));
    }

    /// <summary>
    /// Creates a new milestone under an existing task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<MilestoneResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(MilestoneCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        var result = await _milestoneService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.MilestoneId }, result);
    }

    /// <summary>
    /// Updates an existing milestone.
    /// </summary>
    /// <param name="id">The milestone ID.</param>
    /// <param name="dto">The updated milestone data.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType<MilestoneResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, MilestoneUpdateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _milestoneService.UpdateAsync(id, dto, userId));
    }

    /// <summary>
    /// Deletes a milestone by its ID.
    /// </summary>
    /// <param name="id">The milestone ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        await _milestoneService.DeleteAsync(id, userId);
        return NoContent();
    }

    /// <summary>
    /// Exports all milestones of a task to JSON format.
    /// </summary>
    /// <param name="taskId">The parent task ID.</param>
    [HttpGet("bytask/{taskId:int}/export/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportJson(int taskId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        var data = await _milestoneService.ExportToJsonAsync(taskId, userId);
        return File(data, "application/json", "milestones.json");
    }

    /// <summary>
    /// Exports all milestones of a task to XML format.
    /// </summary>
    /// <param name="taskId">The parent task ID.</param>
    [HttpGet("bytask/{taskId:int}/export/xml")]
    [Produces("application/xml")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportXml(int taskId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        var data = await _milestoneService.ExportToXmlAsync(taskId, userId);
        return File(data, "application/xml", "milestones.xml");
    }

    /// <summary>
    /// Exports all milestones of a task to iCalendar format (.ics) for calendar integration.
    /// </summary>
    /// <param name="taskId">The parent task ID.</param>
    [HttpGet("bytask/{taskId:int}/export/ical")]
    [Produces("text/calendar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportIcal(int taskId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        var data = await _milestoneService.ExportToICalAsync(taskId, userId);
        return File(data, "text/calendar", "milestones.ics");
    }

    /// <summary>
    /// Retrieves the audit history for a specific milestone.
    /// </summary>
    /// <param name="id">The milestone ID.</param>
    [HttpGet("{id:int}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        var history = await _auditLogService.GetMilestoneHistoryAsync(id, userId);
        return Ok(history);
    }
}
