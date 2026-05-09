using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.CalendarEvent;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

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
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CalendarEventsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all calendar events linked to a specific task.
    /// </summary>
    /// <param name="taskId">The parent task ID.</param>
    [HttpGet("bytask/{taskId:int}")]
    [ProducesResponseType<List<CalendarEventResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var events = await _db.CalendarEvents
            .Where(e => e.TaskId == taskId)
            .ToListAsync();

        return Ok(_mapper.Map<List<CalendarEventResponseDto>>(events));
    }

    /// <summary>
    /// Retrieves a single calendar event by its ID.
    /// </summary>
    /// <param name="id">The calendar event ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType<CalendarEventResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var ev = await _db.CalendarEvents.FindAsync(id);
        if (ev is null)
            return NotFound();

        return Ok(_mapper.Map<CalendarEventResponseDto>(ev));
    }

    /// <summary>
    /// Creates a new calendar event linked to an existing task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<CalendarEventResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CalendarEventCreateDto dto)
    {
        var taskExists = await _db.MyTasks.AnyAsync(t => t.MyTaskId == dto.TaskId);
        if (!taskExists)
            return BadRequest("The parent task does not exist.");

        var ev = _mapper.Map<CalendarEvent>(dto);

        _db.CalendarEvents.Add(ev);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = ev.CalendarEventId }, _mapper.Map<CalendarEventResponseDto>(ev));
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
        var ev = await _db.CalendarEvents.FindAsync(id);
        if (ev is null)
            return NotFound();

        _mapper.Map(dto, ev);

        await _db.SaveChangesAsync();

        return Ok(_mapper.Map<CalendarEventResponseDto>(ev));
    }

    /// <summary>
    /// Deletes a calendar event by its ID.
    /// </summary>
    /// <param name="id">The calendar event ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _db.CalendarEvents.FindAsync(id);
        if (ev is null)
            return NotFound();

        _db.CalendarEvents.Remove(ev);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
