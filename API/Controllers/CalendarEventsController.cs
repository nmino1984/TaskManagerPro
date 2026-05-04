using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.CalendarEvent;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendarEventsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CalendarEventsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // GET: api/calendarevents/bytask/5
    [HttpGet("bytask/{taskId:int}")]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var eventsList = await _db.CalendarEvents
            .Where(e => e.TaskId == taskId)
            .ToListAsync();

        var dto = _mapper.Map<List<CalendarEventResponseDto>>(eventsList);
        return Ok(dto);
    }

    // GET: api/calendarevents/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ev = await _db.CalendarEvents.FindAsync(id);
        if (ev is null)
            return NotFound();

        var dto = _mapper.Map<CalendarEventResponseDto>(ev);
        return Ok(dto);
    }

    // POST: api/calendarevents
    [HttpPost]
    public async Task<IActionResult> Create(CalendarEventCreateDto dto)
    {
        // Validate FK
        var taskExists = await _db.MyTasks.AnyAsync(t => t.MyTaskId == dto.TaskId);
        if (!taskExists)
            return BadRequest("The parent MyTask does not exist.");

        var ev = _mapper.Map<CalendarEvent>(dto);

        _db.CalendarEvents.Add(ev);
        await _db.SaveChangesAsync();

        var response = _mapper.Map<CalendarEventResponseDto>(ev);
        return CreatedAtAction(nameof(GetById), new { id = ev.CalendarEventId }, response);
    }

    // PUT: api/calendarevents/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CalendarEventUpdateDto dto)
    {
        var ev = await _db.CalendarEvents.FindAsync(id);
        if (ev is null)
            return NotFound();

        _mapper.Map(dto, ev);

        await _db.SaveChangesAsync();

        var response = _mapper.Map<CalendarEventResponseDto>(ev);
        return Ok(response);
    }

    // DELETE: api/calendarevents/5
    [HttpDelete("{id:int}")]
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
