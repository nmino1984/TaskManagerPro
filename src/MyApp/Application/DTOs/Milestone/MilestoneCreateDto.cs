using MyApp.Domain.Enums;

namespace MyApp.Application.DTOs.Milestone;

public class MilestoneCreateDto
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
    public MilestoneStatus Status { get; set; } = MilestoneStatus.Pending;
}
