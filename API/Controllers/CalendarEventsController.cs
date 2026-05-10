using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.CalendarEvent;
using MyApp.Application.Interfaces;

namespace MyApp.Api.Controllers;

/// <summary>
/// Manages calendar events linked to tasks.
/// </summary>
[ApiController]
[Route("api/v1/calendarevents")]
[Produces("application/json")]
[Consumes("application/json")]
public class CalendarEventsController : ControllerBase
{
    private readonly ICalendarEventService _calendarEventService;

    public CalendarEventsController(ICalendarEventService calendarEventService)
    {
        _calendarEventService = calendarEventService;
    }

    /// <summary>
    /// Retrieves all calendar events linked to a specific task.
    /// </summary>
    /// <param name="taskId">The parent task ID.</param>
    [HttpGet("bytask/{taskId:int}")]
    [ProducesResponseType<List<CalendarEventResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTask(int taskId)
        => Ok(await _calendarEventService.GetByTaskAsync(taskId));

    /// <summary>
    /// Retrieves a single calendar event by its ID.
    /// </summary>
    /// <param name="id">The calendar event ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType<CalendarEventResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _calendarEventService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new calendar event linked to an existing task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<CalendarEventResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CalendarEventCreateDto dto)
    {
        var result = await _calendarEventService.CreateAsync(dto);
        if (result is null)
            return BadRequest("The parent task does not exist.");

        return CreatedAtAction(nameof(GetById), new { id = result.CalendarEventId }, result);
    }

    /// <summary>
    /// Updates an existing calendar event.
    /// </summary>
    /// <param name="id">The calendar event ID.</param>
    /// <param name="dto">The updated calendar event data.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType<CalendarEventResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, CalendarEventUpdateDto dto)
    {
        var result = await _calendarEventService.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Deletes a calendar event by its ID.
    /// </summary>
    /// <param name="id">The calendar event ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
        => await _calendarEventService.DeleteAsync(id) ? NoContent() : NotFound();
}
