using MyApp.Domain.Enums;

namespace MyApp.Application.DTOs.MyTask;

public class MyTaskCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
}
