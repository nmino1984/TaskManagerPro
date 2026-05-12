using MyApp.Domain.Enums;

namespace MyApp.Application.DTOs.CalendarEvent;

public class CalendarEventCreateDto
{
    public int TaskId { get; set; }

    public DateTime Date { get; set; }

    public MyTaskStatus Status { get; set; } = MyTaskStatus.NotStarted;
}
