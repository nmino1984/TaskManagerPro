namespace MyApp.Application.DTOs.CalendarEvent;

public class CalendarEventCreateDto
{
    public int TaskId { get; set; }

    public DateTime Date { get; set; }

    public string Status { get; set; } = "NotStarted";
}
