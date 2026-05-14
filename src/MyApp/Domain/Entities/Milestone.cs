using MyApp.Domain.Enums;

namespace MyApp.Domain.Entities;

public class Milestone
{
    public int MilestoneId { get; set; }
    public int TaskId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime TargetDate { get; set; }
    public MilestoneStatus Status { get; set; } = MilestoneStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public MyTask? Task { get; set; }
}
