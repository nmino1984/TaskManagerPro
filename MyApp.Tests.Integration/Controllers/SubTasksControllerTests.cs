using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MyApp.Application.DTOs.SubTask;
using MyApp.Domain.Enums;
using MyApp.Tests.Integration.Infrastructure;
using Xunit;

namespace MyApp.Tests.Integration.Controllers;

public class SubTasksControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
{
    public SubTasksControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetByTask_Returns200WithSubtasks()
    {
        var task = await SeedTaskAsync();
        await SeedSubTaskAsync(task.MyTaskId, "Subtask A");
        await SeedSubTaskAsync(task.MyTaskId, "Subtask B");

        var response = await Client.GetAsync($"/api/v1/subtasks/bytask/{task.MyTaskId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var subtasks = await response.Content.ReadFromJsonAsync<List<SubTaskResponseDto>>(JsonOptions);
        subtasks!.Should().HaveCountGreaterOrEqualTo(2);
        subtasks.Should().Contain(s => s.Description == "Subtask A");
        subtasks.Should().Contain(s => s.Description == "Subtask B");
    }

    [Fact]
    public async Task GetById_ExistingId_Returns200()
    {
        var task = await SeedTaskAsync();
        var seeded = await SeedSubTaskAsync(task.MyTaskId, "Specific Subtask");

        var response = await Client.GetAsync($"/api/v1/subtasks/{seeded.SubTaskId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var subtask = await response.Content.ReadFromJsonAsync<SubTaskResponseDto>(JsonOptions);
        subtask!.SubTaskId.Should().Be(seeded.SubTaskId);
        subtask.Description.Should().Be("Specific Subtask");
    }

    [Fact]
    public async Task GetById_NonExistingId_Returns404()
    {
        var response = await Client.GetAsync("/api/v1/subtasks/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Not Found");
    }

    [Fact]
    public async Task Create_ValidDto_Returns201WithDefaultPendingStatus()
    {
        var task = await SeedTaskAsync();
        var body = new SubTaskCreateDto
        {
            TaskId = task.MyTaskId,
            Description = "New subtask",
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var response = await Client.PostAsJsonAsync("/api/v1/subtasks", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<SubTaskResponseDto>(JsonOptions);
        created!.TaskId.Should().Be(task.MyTaskId);
        created.Description.Should().Be("New subtask");
        created.Status.Should().Be(SubTaskStatus.Pending);
    }

    [Fact]
    public async Task Create_NonExistingTaskId_Returns404()
    {
        var body = new SubTaskCreateDto { TaskId = 99999, Description = "Orphan subtask" };

        var response = await Client.PostAsJsonAsync("/api/v1/subtasks", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_EmptyDescription_Returns400()
    {
        var task = await SeedTaskAsync();
        var body = new SubTaskCreateDto { TaskId = task.MyTaskId, Description = "" };

        var response = await Client.PostAsJsonAsync("/api/v1/subtasks", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Description");
    }

    [Fact]
    public async Task Create_CompletedSubtask_SyncsParentTaskProgress()
    {
        var task = await SeedTaskAsync("Progress Sync Task");
        var body = new SubTaskCreateDto
        {
            TaskId = task.MyTaskId,
            Description = "Only subtask",
            Status = SubTaskStatus.Completed
        };

        await Client.PostAsJsonAsync("/api/v1/subtasks", body, JsonOptions);

        var taskResponse = await Client.GetAsync($"/api/v1/tasks/{task.MyTaskId}");
        var updatedTask = await taskResponse.Content.ReadFromJsonAsync<
            MyApp.Application.DTOs.MyTask.MyTaskResponseDto>(JsonOptions);
        updatedTask!.Progress.Should().Be(100);
        updatedTask.Status.Should().Be(MyTaskStatus.Completed);
    }

    [Fact]
    public async Task Update_ExistingId_Returns200WithNewValues()
    {
        var task = await SeedTaskAsync();
        var seeded = await SeedSubTaskAsync(task.MyTaskId);
        var body = new SubTaskUpdateDto
        {
            Description = "Updated description",
            Status = SubTaskStatus.Completed,
            Notes = "Done!"
        };

        var response = await Client.PutAsJsonAsync($"/api/v1/subtasks/{seeded.SubTaskId}", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<SubTaskResponseDto>(JsonOptions);
        updated!.Description.Should().Be("Updated description");
        updated.Status.Should().Be(SubTaskStatus.Completed);
    }

    [Fact]
    public async Task Update_NonExistingId_Returns404()
    {
        var body = new SubTaskUpdateDto { Description = "Ghost", Status = SubTaskStatus.Pending };

        var response = await Client.PutAsJsonAsync("/api/v1/subtasks/99999", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ExistingId_Returns204AndSubtaskIsGone()
    {
        var task = await SeedTaskAsync();
        var seeded = await SeedSubTaskAsync(task.MyTaskId);

        var deleteResponse = await Client.DeleteAsync($"/api/v1/subtasks/{seeded.SubTaskId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/v1/subtasks/{seeded.SubTaskId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NonExistingId_Returns404()
    {
        var response = await Client.DeleteAsync("/api/v1/subtasks/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
