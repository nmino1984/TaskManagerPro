using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.SubTask;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

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
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public SubTasksController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all subtasks belonging to a specific task.
    /// </summary>
    /// <param name="taskId">The parent task ID.</param>
    [HttpGet("bytask/{taskId:int}")]
    [ProducesResponseType<List<SubTaskResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var subtasks = await _db.SubTasks
            .Where(s => s.TaskId == taskId)
            .ToListAsync();

        return Ok(_mapper.Map<List<SubTaskResponseDto>>(subtasks));
    }

    /// <summary>
    /// Retrieves a single subtask by its ID.
    /// </summary>
    /// <param name="id">The subtask ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType<SubTaskResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var subtask = await _db.SubTasks.FindAsync(id);
        if (subtask is null)
            return NotFound();

        return Ok(_mapper.Map<SubTaskResponseDto>(subtask));
    }

    /// <summary>
    /// Creates a new subtask under an existing task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<SubTaskResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(SubTaskCreateDto dto)
    {
        var taskExists = await _db.MyTasks.AnyAsync(t => t.MyTaskId == dto.TaskId);
        if (!taskExists)
            return BadRequest("The parent task does not exist.");

        var subtask = _mapper.Map<SubTask>(dto);
        subtask.CreatedAt = DateTime.UtcNow;
        subtask.UpdatedAt = DateTime.UtcNow;

        _db.SubTasks.Add(subtask);
        await _db.SaveChangesAsync();

        await SyncTaskProgressAsync(dto.TaskId);

        return CreatedAtAction(nameof(GetById), new { id = subtask.SubTaskId }, _mapper.Map<SubTaskResponseDto>(subtask));
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
        var subtask = await _db.SubTasks.FindAsync(id);
        if (subtask is null)
            return NotFound();

        _mapper.Map(dto, subtask);
        subtask.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await SyncTaskProgressAsync(subtask.TaskId);

        return Ok(_mapper.Map<SubTaskResponseDto>(subtask));
    }

    /// <summary>
    /// Deletes a subtask by its ID.
    /// </summary>
    /// <param name="id">The subtask ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var subtask = await _db.SubTasks.FindAsync(id);
        if (subtask is null)
            return NotFound();

        var taskId = subtask.TaskId;

        _db.SubTasks.Remove(subtask);
        await _db.SaveChangesAsync();

        await SyncTaskProgressAsync(taskId);

        return NoContent();
    }

    private async Task SyncTaskProgressAsync(int taskId)
    {
        var task = await _db.MyTasks
            .Include(t => t.SubTasks)
            .FirstOrDefaultAsync(t => t.MyTaskId == taskId);

        if (task is null) return;

        task.UpdateProgress();
        task.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
