using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.SubTask;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubTasksController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public SubTasksController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // GET: api/subtasks/bytask/5
    [HttpGet("bytask/{taskId:int}")]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var subtasks = await _db.SubTasks
            .Where(s => s.TaskId == taskId)
            .ToListAsync();

        var dto = _mapper.Map<List<SubTaskResponseDto>>(subtasks);
        return Ok(dto);
    }

    // GET: api/subtasks/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var subtask = await _db.SubTasks.FindAsync(id);
        if (subtask is null)
            return NotFound();

        var dto = _mapper.Map<SubTaskResponseDto>(subtask);
        return Ok(dto);
    }

    // POST: api/subtasks
    [HttpPost]
    public async Task<IActionResult> Create(SubTaskCreateDto dto)
    {
        // Validate FK
        var taskExists = await _db.MyTasks.AnyAsync(t => t.MyTaskId == dto.TaskId);
        if (!taskExists)
            return BadRequest("The parent MyTask does not exist.");

        var subtask = _mapper.Map<SubTask>(dto);

        subtask.CreatedAt = DateTime.UtcNow;
        subtask.UpdatedAt = DateTime.UtcNow;

        _db.SubTasks.Add(subtask);
        await _db.SaveChangesAsync();

        var response = _mapper.Map<SubTaskResponseDto>(subtask);
        return CreatedAtAction(nameof(GetById), new { id = subtask.SubTaskId }, response);
    }

    // PUT: api/subtasks/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SubTaskUpdateDto dto)
    {
        var subtask = await _db.SubTasks.FindAsync(id);
        if (subtask is null)
            return NotFound();

        _mapper.Map(dto, subtask);

        subtask.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var response = _mapper.Map<SubTaskResponseDto>(subtask);
        return Ok(response);
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
