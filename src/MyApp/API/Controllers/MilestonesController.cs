using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.Milestone;
using MyApp.Application.Interfaces;

namespace MyApp.Api.Controllers;

/// <summary>
/// Manages milestones (important objectives) belonging to a parent task.
/// Milestones represent key checkpoints like code reviews, deliveries, or validations.
/// </summary>
[ApiController]
[Route("api/v1/milestones")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class MilestonesController : ControllerBase
{
    private readonly IMilestoneService _milestoneService;

    public MilestonesController(IMilestoneService milestoneService)
    {
        _milestoneService = milestoneService;
    }

    /// <summary>
    /// Retrieves all milestones belonging to a specific task.
    /// </summary>
    /// <param name="taskId">The parent task ID.</param>
    [HttpGet("bytask/{taskId:int}")]
    [ProducesResponseType<List<MilestoneResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTask(int taskId)
        => Ok(await _milestoneService.GetByTaskAsync(taskId));

    /// <summary>
    /// Retrieves a single milestone by its ID.
    /// </summary>
    /// <param name="id">The milestone ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType<MilestoneResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _milestoneService.GetByIdAsync(id));

    /// <summary>
    /// Creates a new milestone under an existing task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<MilestoneResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(MilestoneCreateDto dto)
    {
        var result = await _milestoneService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.MilestoneId }, result);
    }

    /// <summary>
    /// Updates an existing milestone.
    /// </summary>
    /// <param name="id">The milestone ID.</param>
    /// <param name="dto">The updated milestone data.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType<MilestoneResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, MilestoneUpdateDto dto)
        => Ok(await _milestoneService.UpdateAsync(id, dto));

    /// <summary>
    /// Deletes a milestone by its ID.
    /// </summary>
    /// <param name="id">The milestone ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _milestoneService.DeleteAsync(id);
        return NoContent();
    }
}
