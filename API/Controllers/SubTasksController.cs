using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure.Data;
using MyApp.Domain.Entities;

namespace MyApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubTasksController : ControllerBase
{
    private readonly AppDbContext _db;

    public SubTasksController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/subtasks
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subtasks = await _db.SubTasks.ToListAsync();
        return Ok(subtasks);
    }

    // GET: api/subtasks/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var subtask = await _db.SubTasks.FindAsync(id);
        if (subtask is null)
            return NotFound();

        return Ok(subtask);
    }

    // GET: api/subtasks/bytask/3
    [HttpGet("bytask/{taskId:int}")]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var subtasks = await _db.SubTasks
            .Where(s => s.TaskId == taskId)
            .ToListAsync();

        return Ok(subtasks);
    }

    // POST: api/subtasks
    [HttpPost]
    public async Task<IActionResult> Create(SubTask subtask)
    {
        // Validate FK
        var taskExists = await _db.MyTasks.AnyAsync(t => t.MyTaskId == subtask.TaskId);
        if (!taskExists)
            return BadRequest("The parent MyTask does not exist.");

        _db.SubTasks.Add(subtask);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = subtask.SubTaskId }, subtask);
    }

    // PUT: api/subtasks/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SubTask updated)
    {
        var subtask = await _db.SubTasks.FindAsync(id);
        if (subtask is null)
            return NotFound();

        subtask.Description = updated.Description;
        subtask.Status = updated.Status;

        await _db.SaveChangesAsync();
        return Ok(subtask);
    }

    // DELETE: api/subtasks/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var subtask = await _db.SubTasks.FindAsync(id);
        if (subtask is null)
            return NotFound();

        _db.SubTasks.Remove(subtask);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
