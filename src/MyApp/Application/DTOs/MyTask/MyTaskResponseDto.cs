using MyApp.Application.DTOs.Milestone;
using MyApp.Application.DTOs.SubTask;
using MyApp.Domain.Enums;

namespace MyApp.Application.DTOs.MyTask;

public class MyTaskResponseDto
{
    public int MyTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public TaskPriority Priority { get; set; }
    public MyTaskStatus Status { get; set; }
    public int Progress { get; set; }

    public string? AssignedToUserId { get; set; }
    public string? AssignedToUsername { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<SubTaskResponseDto>? SubTasks { get; set; }
    public List<MilestoneResponseDto>? Milestones { get; set; }
}
