using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MyApp.Application.DTOs.Common;
using MyApp.Application.DTOs.MyTask;
using MyApp.Domain.Enums;
using MyApp.Tests.Integration.Infrastructure;
using Xunit;

namespace MyApp.Tests.Integration.Controllers;

public class TasksControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
{
    public TasksControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_Returns200_ContainsSeededTask()
    {
        var seeded = await SeedTaskAsync("GetAll Unique Title");

        var response = await Client.GetAsync("/api/v1/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);
        result.Should().NotBeNull();
        result!.Items.Should().Contain(t => t.MyTaskId == seeded.MyTaskId && t.Title == "GetAll Unique Title");
        result.TotalCount.Should().BeGreaterThan(0);
        result.Page.Should().Be(1);
    }

    [Fact]
    public async Task GetById_ExistingId_Returns200WithCorrectTask()
    {
        var seeded = await SeedTaskAsync("GetById Target Task", TaskPriority.High);

        var response = await Client.GetAsync($"/api/v1/tasks/{seeded.MyTaskId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var task = await response.Content.ReadFromJsonAsync<MyTaskResponseDto>(JsonOptions);
        task!.MyTaskId.Should().Be(seeded.MyTaskId);
        task.Title.Should().Be("GetById Target Task");
        task.Priority.Should().Be(TaskPriority.High);
    }

    [Fact]
    public async Task GetById_NonExistingId_Returns404ProblemDetails()
    {
        await AuthenticateAsync();
        var response = await Client.GetAsync("/api/v1/tasks/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Not Found");
        body.Should().Contain("99999");
    }

    [Fact]
    public async Task Create_ValidDto_Returns201WithLocationAndBody()
    {
        await AuthenticateAsync();
        var body = new MyTaskCreateDto
        {
            Title = "Created Task",
            Description = "A well-formed task",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(15),
            Priority = TaskPriority.High
        };

        var response = await Client.PostAsJsonAsync("/api/v1/tasks", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var created = await response.Content.ReadFromJsonAsync<MyTaskResponseDto>(JsonOptions);
        created!.Title.Should().Be("Created Task");
        created.Priority.Should().Be(TaskPriority.High);
        created.Status.Should().Be(MyTaskStatus.NotStarted);
        created.Progress.Should().Be(0);
        created.MyTaskId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Create_EmptyTitle_Returns400WithValidationError()
    {
        await AuthenticateAsync();
        var body = new MyTaskCreateDto
        {
            Title = "",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            Priority = TaskPriority.Medium
        };

        var response = await Client.PostAsJsonAsync("/api/v1/tasks", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body2 = await response.Content.ReadAsStringAsync();
        body2.Should().Contain("Title");
    }

    [Fact]
    public async Task Create_EndDateBeforeStartDate_Returns400WithValidationError()
    {
        await AuthenticateAsync();
        var body = new MyTaskCreateDto
        {
            Title = "Bad dates",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow,
            Priority = TaskPriority.Low
        };

        var response = await Client.PostAsJsonAsync("/api/v1/tasks", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("StartDate");
    }

    [Fact]
    public async Task Update_ExistingId_Returns200WithUpdatedData()
    {
        var seeded = await SeedTaskAsync("Before Update");
        var updateBody = new MyTaskUpdateDto
        {
            Title = "After Update",
            Description = "Updated description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(20),
            Priority = TaskPriority.High,
            Status = MyTaskStatus.InProgress
        };

        var response = await Client.PutAsJsonAsync($"/api/v1/tasks/{seeded.MyTaskId}", updateBody, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<MyTaskResponseDto>(JsonOptions);
        updated!.Title.Should().Be("After Update");
        updated.Priority.Should().Be(TaskPriority.High);
    }

    [Fact]
    public async Task Update_NonExistingId_Returns404()
    {
        await AuthenticateAsync();
        var body = new MyTaskUpdateDto
        {
            Title = "Ghost",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            Priority = TaskPriority.Low,
            Status = MyTaskStatus.NotStarted
        };

        var response = await Client.PutAsJsonAsync("/api/v1/tasks/99999", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ExistingId_Returns204AndTaskIsGone()
    {
        var seeded = await SeedTaskAsync("Task to delete");

        var deleteResponse = await Client.DeleteAsync($"/api/v1/tasks/{seeded.MyTaskId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/v1/tasks/{seeded.MyTaskId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NonExistingId_Returns404()
    {
        await AuthenticateAsync();
        var response = await Client.DeleteAsync("/api/v1/tasks/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ExistingId_TaskIsHiddenToAPI()
    {
        var seeded = await SeedTaskAsync("Task to soft-delete");

        var deleteResponse = await Client.DeleteAsync($"/api/v1/tasks/{seeded.MyTaskId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/v1/tasks/{seeded.MyTaskId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var listResponse = await Client.GetAsync("/api/v1/tasks");
        var list = await listResponse.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);
        list!.Items.Should().NotContain(t => t.MyTaskId == seeded.MyTaskId);
    }
}
