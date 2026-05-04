namespace MyApp.Domain.Entities;

public class CalendarEvent
{
    public int CalendarEventId { get; set; }
    public int TaskId { get; set; }

    public DateTime Date { get; set; }

    // Allowed values: same as Task.Status
    public string Status { get; set; } = "NotStarted";

    public MyTask? Task { get; set; }
}
