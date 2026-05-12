using MyApp.Application.DTOs.CalendarEvent;

namespace MyApp.Application.Interfaces;

public interface ICalendarEventService
{
    Task<List<CalendarEventResponseDto>> GetByTaskAsync(int taskId);
    Task<CalendarEventResponseDto> GetByIdAsync(int id);
    Task<CalendarEventResponseDto> CreateAsync(CalendarEventCreateDto dto);
    Task<CalendarEventResponseDto> UpdateAsync(int id, CalendarEventUpdateDto dto);
    Task DeleteAsync(int id);
}
