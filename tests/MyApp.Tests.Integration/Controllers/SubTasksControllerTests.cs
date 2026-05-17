using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MyApp.Application.DTOs.MyTask;
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
        await AuthenticateAsync();
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
        await AuthenticateAsync();
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
        await AuthenticateAsync();
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
        await AuthenticateAsync();
        var response = await Client.DeleteAsync("/api/v1/subtasks/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ===== AUTHENTICATION TESTS =====

    [Fact]
    public async Task GetByTask_WithoutAuthorizationHeader_Returns401Unauthorized()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/v1/subtasks/bytask/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithoutAuthorizationHeader_Returns401Unauthorized()
    {
        var client = Factory.CreateClient();
        var body = new SubTaskCreateDto { TaskId = 1, Description = "Unauthorized" };

        var response = await client.PostAsJsonAsync("/api/v1/subtasks", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ===== MULTI-TENANCY TESTS =====

    [Fact]
    public async Task GetByTask_DifferentUserCannotAccessOthersSubtasks()
    {
        // User 1: Create task and subtask
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");
        var user1Subtask = await CreateSubtaskViaClient(user1Client, user1Task.MyTaskId, "User1 Subtask");

        // User 2: Try to get User 1's subtasks
        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var response = await user2Client.GetAsync($"/api/v1/subtasks/bytask/{user1Task.MyTaskId}");

        // Should return 404 (task not found from user 2's perspective)
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_DifferentUserCannotAccessOthersSubtask()
    {
        // User 1: Create task and subtask
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");
        var user1Subtask = await CreateSubtaskViaClient(user1Client, user1Task.MyTaskId, "User1 Subtask");

        // User 2: Try to get User 1's subtask
        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var response = await user2Client.GetAsync($"/api/v1/subtasks/{user1Subtask.SubTaskId}");

        // Should return 404
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_CannotCreateSubtaskForOthersTask()
    {
        // User 1: Create task
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");

        // User 2: Try to create subtask under User 1's task
        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var body = new SubTaskCreateDto
        {
            TaskId = user1Task.MyTaskId,
            Description = "User2 tries to hijack User1's task"
        };

        var response = await user2Client.PostAsJsonAsync("/api/v1/subtasks", body, JsonOptions);

        // Should fail with 404 (task not found from user 2's perspective)
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DifferentUserCannotModifyOthersSubtask()
    {
        // User 1: Create task and subtask
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");
        var user1Subtask = await CreateSubtaskViaClient(user1Client, user1Task.MyTaskId, "Original Subtask");

        // User 2: Try to update User 1's subtask
        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var updateBody = new SubTaskUpdateDto
        {
            Description = "Hacked subtask",
            Status = SubTaskStatus.Completed,
            Notes = "Malicious edit"
        };

        var response = await user2Client.PutAsJsonAsync($"/api/v1/subtasks/{user1Subtask.SubTaskId}", updateBody, JsonOptions);

        // Should return 404
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Verify original is unchanged
        var verifyResponse = await user1Client.GetAsync($"/api/v1/subtasks/{user1Subtask.SubTaskId}");
        var original = await verifyResponse.Content.ReadFromJsonAsync<SubTaskResponseDto>(JsonOptions);
        original!.Description.Should().Be("Original Subtask");
        original.Status.Should().Be(SubTaskStatus.Pending);
    }

    [Fact]
    public async Task Delete_DifferentUserCannotDeleteOthersSubtask()
    {
        // User 1: Create task and subtask
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");
        var user1Subtask = await CreateSubtaskViaClient(user1Client, user1Task.MyTaskId, "Protected Subtask");

        // User 2: Try to delete User 1's subtask
        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var deleteResponse = await user2Client.DeleteAsync($"/api/v1/subtasks/{user1Subtask.SubTaskId}");

        // Should return 404
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Verify subtask still exists for User 1
        var verifyResponse = await user1Client.GetAsync($"/api/v1/subtasks/{user1Subtask.SubTaskId}");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ===== HELPER METHODS =====

    private async Task<MyTaskResponseDto> CreateTaskViaClient(HttpClient client, string title)
    {
        var body = new MyTaskCreateDto
        {
            Title = title,
            Description = "Test task",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            Priority = TaskPriority.Medium
        };
        var response = await client.PostAsJsonAsync("/api/v1/tasks", body, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MyTaskResponseDto>(JsonOptions))!;
    }

    private async Task<SubTaskResponseDto> CreateSubtaskViaClient(HttpClient client, int taskId, string description)
    {
        var body = new SubTaskCreateDto
        {
            TaskId = taskId,
            Description = description
        };
        var response = await client.PostAsJsonAsync("/api/v1/subtasks", body, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SubTaskResponseDto>(JsonOptions))!;
    }
}
