using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure.Data;
using MyApp.Domain.Entities;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendarEventsController : ControllerBase
{
    private readonly AppDbContext _db;

    public CalendarEventsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/calendarevents
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var eventsList = await _db.CalendarEvents.ToListAsync();
        return Ok(eventsList);
    }

    // GET: api/calendarevents/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ev = await _db.CalendarEvents.FindAsync(id);
        if (ev is null)
            return NotFound();

        return Ok(ev);
    }

    // GET: api/calendarevents/bytask/3
    [HttpGet("bytask/{taskId:int}")]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var eventsList = await _db.CalendarEvents
            .Where(e => e.TaskId == taskId)
            .ToListAsync();

        return Ok(eventsList);
    }

    // POST: api/calendarevents
    [HttpPost]
    public async Task<IActionResult> Create(CalendarEvent ev)
    {
        // Validate FK
        var taskExists = await _db.MyTasks.AnyAsync(t => t.MyTaskId == ev.TaskId);
        if (!taskExists)
            return BadRequest("The parent MyTask does not exist.");

        _db.CalendarEvents.Add(ev);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = ev.CalendarEventId }, ev);
    }

    // PUT: api/calendarevents/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CalendarEvent updated)
    {
        var ev = await _db.CalendarEvents.FindAsync(id);
        if (ev is null)
            return NotFound();

        ev.Date = updated.Date;
        ev.Status = updated.Status;
        ev.TaskId = updated.TaskId;

        await _db.SaveChangesAsync();
        return Ok(ev);
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
