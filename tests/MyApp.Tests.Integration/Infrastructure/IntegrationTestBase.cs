using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MyApp.Application.DTOs.CalendarEvent;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.DTOs.SubTask;
using MyApp.Domain.Enums;

namespace MyApp.Tests.Integration.Infrastructure;

public abstract class IntegrationTestBase
{
    protected readonly HttpClient Client;

    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected async Task<MyTaskResponseDto> SeedTaskAsync(
        string title = "Seed Task",
        TaskPriority priority = TaskPriority.Medium,
        int daysUntilEnd = 30)
    {
        var body = new MyTaskCreateDto
        {
            Title = title,
            Description = "Seeded for integration testing",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(daysUntilEnd),
            Priority = priority
        };
        var response = await Client.PostAsJsonAsync("/api/v1/tasks", body, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MyTaskResponseDto>(JsonOptions))!;
    }

    protected async Task<SubTaskResponseDto> SeedSubTaskAsync(int taskId, string description = "Seed SubTask")
    {
        var body = new SubTaskCreateDto { TaskId = taskId, Description = description };
        var response = await Client.PostAsJsonAsync("/api/v1/subtasks", body, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SubTaskResponseDto>(JsonOptions))!;
    }

    protected async Task<CalendarEventResponseDto> SeedCalendarEventAsync(int taskId)
    {
        var body = new CalendarEventCreateDto { TaskId = taskId, Date = DateTime.UtcNow.AddDays(5) };
        var response = await Client.PostAsJsonAsync("/api/v1/calendarevents", body, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CalendarEventResponseDto>(JsonOptions))!;
    }
}
