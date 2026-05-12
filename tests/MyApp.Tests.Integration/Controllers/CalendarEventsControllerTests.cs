using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MyApp.Application.DTOs.CalendarEvent;
using MyApp.Domain.Enums;
using MyApp.Tests.Integration.Infrastructure;
using Xunit;

namespace MyApp.Tests.Integration.Controllers;

public class CalendarEventsControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
{
    public CalendarEventsControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetByTask_Returns200WithEvents()
    {
        var task = await SeedTaskAsync();
        await SeedCalendarEventAsync(task.MyTaskId);
        await SeedCalendarEventAsync(task.MyTaskId);

        var response = await Client.GetAsync($"/api/v1/calendarevents/bytask/{task.MyTaskId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var events = await response.Content.ReadFromJsonAsync<List<CalendarEventResponseDto>>(JsonOptions);
        events!.Should().HaveCountGreaterOrEqualTo(2);
        events.Should().AllSatisfy(e => e.TaskId.Should().Be(task.MyTaskId));
    }

    [Fact]
    public async Task GetById_ExistingId_Returns200()
    {
        var task = await SeedTaskAsync();
        var seeded = await SeedCalendarEventAsync(task.MyTaskId);

        var response = await Client.GetAsync($"/api/v1/calendarevents/{seeded.CalendarEventId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var ev = await response.Content.ReadFromJsonAsync<CalendarEventResponseDto>(JsonOptions);
        ev!.CalendarEventId.Should().Be(seeded.CalendarEventId);
        ev.TaskId.Should().Be(task.MyTaskId);
    }

    [Fact]
    public async Task GetById_NonExistingId_Returns404()
    {
        await AuthenticateAsync();
        var response = await Client.GetAsync("/api/v1/calendarevents/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Not Found");
    }

    [Fact]
    public async Task Create_ValidDto_Returns201WithDefaultNotStartedStatus()
    {
        var task = await SeedTaskAsync();
        var body = new CalendarEventCreateDto
        {
            TaskId = task.MyTaskId,
            Date = DateTime.UtcNow.AddDays(3)
        };

        var response = await Client.PostAsJsonAsync("/api/v1/calendarevents", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CalendarEventResponseDto>(JsonOptions);
        created!.TaskId.Should().Be(task.MyTaskId);
        created.Status.Should().Be(MyTaskStatus.NotStarted);
        created.CalendarEventId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Create_NonExistingTaskId_Returns404()
    {
        await AuthenticateAsync();
        var body = new CalendarEventCreateDto { TaskId = 99999, Date = DateTime.UtcNow.AddDays(3) };

        var response = await Client.PostAsJsonAsync("/api/v1/calendarevents", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ExistingId_Returns200WithNewValues()
    {
        var task = await SeedTaskAsync();
        var seeded = await SeedCalendarEventAsync(task.MyTaskId);
        var newDate = DateTime.UtcNow.AddDays(15);
        var body = new CalendarEventUpdateDto
        {
            Date = newDate,
            Status = MyTaskStatus.InProgress
        };

        var response = await Client.PutAsJsonAsync($"/api/v1/calendarevents/{seeded.CalendarEventId}", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<CalendarEventResponseDto>(JsonOptions);
        updated!.Status.Should().Be(MyTaskStatus.InProgress);
        updated.Date.Date.Should().Be(newDate.Date);
    }

    [Fact]
    public async Task Update_NonExistingId_Returns404()
    {
        await AuthenticateAsync();
        var body = new CalendarEventUpdateDto { Date = DateTime.UtcNow.AddDays(5), Status = MyTaskStatus.NotStarted };

        var response = await Client.PutAsJsonAsync("/api/v1/calendarevents/99999", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ExistingId_Returns204AndEventIsGone()
    {
        var task = await SeedTaskAsync();
        var seeded = await SeedCalendarEventAsync(task.MyTaskId);

        var deleteResponse = await Client.DeleteAsync($"/api/v1/calendarevents/{seeded.CalendarEventId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/v1/calendarevents/{seeded.CalendarEventId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NonExistingId_Returns404()
    {
        await AuthenticateAsync();
        var response = await Client.DeleteAsync("/api/v1/calendarevents/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
