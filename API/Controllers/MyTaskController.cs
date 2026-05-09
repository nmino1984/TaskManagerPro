using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.MyTask;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Api.Controllers;

/// <summary>
/// Manages tasks including their subtasks and calendar events.
/// </summary>
[ApiController]
[Route("api/v1/tasks")]
[Produces("application/json")]
[Consumes("application/json")]
public class MyTaskController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public MyTaskController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all tasks including their subtasks and calendar events.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<List<MyTaskResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MyTaskResponseDto>>> GetAll()
    {
        var tasks = await _db.MyTasks
            .Include(t => t.SubTasks)
            .Include(t => t.CalendarEvents)
            .ToListAsync();

        return Ok(_mapper.Map<List<MyTaskResponseDto>>(tasks));
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
        var task = await _db.MyTasks
            .Include(t => t.SubTasks)
            .Include(t => t.CalendarEvents)
            .FirstOrDefaultAsync(t => t.MyTaskId == id);

        if (task is null)
            return NotFound();

        return Ok(_mapper.Map<MyTaskResponseDto>(task));
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<MyTaskResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MyTaskResponseDto>> Create(MyTaskCreateDto dto)
    {
        var task = _mapper.Map<MyTask>(dto);

        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _db.MyTasks.Add(task);
        await _db.SaveChangesAsync();

        var response = _mapper.Map<MyTaskResponseDto>(task);

        return CreatedAtAction(nameof(GetById), new { id = task.MyTaskId }, response);
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
        var task = await _db.MyTasks.FindAsync(id);
        if (task is null)
            return NotFound();

        _mapper.Map(dto, task);
        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(_mapper.Map<MyTaskResponseDto>(task));
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
        var task = await _db.MyTasks.FindAsync(id);
        if (task is null)
            return NotFound();

        _db.MyTasks.Remove(task);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
