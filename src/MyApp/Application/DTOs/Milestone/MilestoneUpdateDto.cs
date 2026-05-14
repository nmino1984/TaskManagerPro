using MyApp.Domain.Enums;

namespace MyApp.Application.DTOs.Milestone;

public class MilestoneUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? TargetDate { get; set; }
    public MilestoneStatus? Status { get; set; }
}
