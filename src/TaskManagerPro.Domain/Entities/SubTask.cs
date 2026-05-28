using System;
using TaskManagerPro.Domain.Enums;

namespace TaskManagerPro.Domain.Entities;

public class SubTask
{
    public int SubTaskId { get; set; }
    public int TaskId { get; set; }

    public string Description { get; set; } = string.Empty;

    public SubTaskStatus Status { get; set; } = SubTaskStatus.Pending;

    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
