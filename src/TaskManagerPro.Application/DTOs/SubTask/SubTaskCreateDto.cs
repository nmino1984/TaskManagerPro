using System;
using TaskManagerPro.Domain.Enums;

namespace TaskManagerPro.Application.DTOs.SubTask;

public class SubTaskCreateDto
{
    public int TaskId { get; set; }

    public string Description { get; set; } = string.Empty;

    public SubTaskStatus Status { get; set; } = SubTaskStatus.Pending;

    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
}
