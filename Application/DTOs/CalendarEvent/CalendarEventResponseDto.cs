namespace MyApp.Application.DTOs.CalendarEvent;

public class CalendarEventResponseDto
{
    public int CalendarEventId { get; set; }
    public int TaskId { get; set; }

    public DateTime Date { get; set; }
    public string Status { get; set; } = "NotStarted";
}
