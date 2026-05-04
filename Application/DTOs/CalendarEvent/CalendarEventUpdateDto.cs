namespace MyApp.Application.DTOs.CalendarEvent;

public class CalendarEventUpdateDto
{
    public DateTime Date { get; set; }

    public string Status { get; set; } = "NotStarted";
}
