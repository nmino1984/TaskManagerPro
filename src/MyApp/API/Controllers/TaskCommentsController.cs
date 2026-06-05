using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.TaskComment;
using MyApp.Application.Interfaces;
using System.Security.Claims;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/v1/comments")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class TaskCommentsController : ControllerBase
{
    private readonly ITaskCommentService _commentService;

    public TaskCommentsController(ITaskCommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("bytask/{taskId:int}")]
    [ProducesResponseType<List<TaskCommentResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _commentService.GetByTaskAsync(taskId, userId));
    }

    [HttpPost]
    [ProducesResponseType<TaskCommentResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(TaskCommentCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        var result = await _commentService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetByTask), new { taskId = result.TaskId }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<TaskCommentResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, TaskCommentUpdateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        return Ok(await _commentService.UpdateAsync(id, dto, userId));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        await _commentService.DeleteAsync(id, userId);
        return NoContent();
    }
}
