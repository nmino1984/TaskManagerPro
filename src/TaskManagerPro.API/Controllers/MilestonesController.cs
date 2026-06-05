using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerPro.Application.DTOs.Milestone;
using TaskManagerPro.Application.Interfaces;

namespace TaskManagerPro.API.Controllers;

/// <summary>
/// Manages milestones belonging to a parent task.
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

    [HttpGet("bytask/{taskId:int}")]
    [ProducesResponseType<List<MilestoneResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _milestoneService.GetByTaskAsync(taskId, userId));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<MilestoneResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _milestoneService.GetByIdAsync(id, userId));
    }

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
    /// Exports all milestones of a task to iCalendar format.
    /// </summary>
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
    [HttpGet("{id:int}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _auditLogService.GetMilestoneHistoryAsync(id, userId));
    }
}
