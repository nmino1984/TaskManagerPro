using MyApp.Domain.Enums;

namespace MyApp.Application.DTOs.CalendarEvent;

public class CalendarEventUpdateDto
{
    public DateTime Date { get; set; }

    public MyTaskStatus Status { get; set; } = MyTaskStatus.NotStarted;
}
