using MyApp.Application.DTOs.CalendarEvent;
using MyApp.Application.DTOs.SubTask;

namespace MyApp.Application.DTOs.MyTask;

public class MyTaskResponseDto
{
    public int MyTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "NotStarted";
    public int Progress { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<SubTaskResponseDto>? SubTasks { get; set; }
    public List<CalendarEventResponseDto>? CalendarEvents { get; set; }
}
