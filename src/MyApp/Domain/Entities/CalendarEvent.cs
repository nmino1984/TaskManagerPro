using MyApp.Domain.Enums;

namespace MyApp.Domain.Entities;

public class CalendarEvent
{
    public int CalendarEventId { get; set; }
    public int TaskId { get; set; }

    public DateTime Date { get; set; }

    public MyTaskStatus Status { get; set; } = MyTaskStatus.NotStarted;

    public MyTask? Task { get; set; }
}
