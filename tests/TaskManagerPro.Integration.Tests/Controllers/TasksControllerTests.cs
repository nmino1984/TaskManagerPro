using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerPro.Application.DTOs.Common;
using TaskManagerPro.Application.DTOs.MyTask;
using TaskManagerPro.Domain.Enums;
using TaskManagerPro.Integration.Tests.Infrastructure;
using Xunit;

namespace TaskManagerPro.Integration.Tests.Controllers;

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

    // ===== AUTHENTICATION TESTS =====

    [Fact]
    public async Task GetAll_WithoutAuthorizationHeader_Returns401Unauthorized()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/v1/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_WithInvalidToken_Returns401Unauthorized()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer invalid-token-xyz");

        var response = await client.GetAsync("/api/v1/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ===== MULTI-TENANCY TESTS =====

    [Fact]
    public async Task GetById_DifferentUserCannotAccessTask()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var createBody = new MyTaskCreateDto
        {
            Title = "User1 Private Task",
            Description = "Only User1 should see this",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            Priority = TaskPriority.High
        };
        var createResponse = await user1Client.PostAsJsonAsync("/api/v1/tasks", createBody, JsonOptions);
        var user1Task = await createResponse.Content.ReadFromJsonAsync<MyTaskResponseDto>(JsonOptions);

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var getResponse = await user2Client.GetAsync($"/api/v1/tasks/{user1Task!.MyTaskId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_EachUserSeesOnlyOwnTasks()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task1 = await CreateTaskViaClient(user1Client, "User1 Task 1");
        var user1Task2 = await CreateTaskViaClient(user1Client, "User1 Task 2");

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var user2Task1 = await CreateTaskViaClient(user2Client, "User2 Task 1");

        var user1Response = await user1Client.GetAsync("/api/v1/tasks");
        var user1Result = await user1Response.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);

        user1Result!.Items.Should().Contain(t => t.MyTaskId == user1Task1.MyTaskId);
        user1Result.Items.Should().Contain(t => t.MyTaskId == user1Task2.MyTaskId);
        user1Result.Items.Should().NotContain(t => t.MyTaskId == user2Task1.MyTaskId);

        var user2Response = await user2Client.GetAsync("/api/v1/tasks");
        var user2Result = await user2Response.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);

        user2Result!.Items.Should().Contain(t => t.MyTaskId == user2Task1.MyTaskId);
        user2Result.Items.Should().NotContain(t => t.MyTaskId == user1Task1.MyTaskId);
        user2Result.Items.Should().NotContain(t => t.MyTaskId == user1Task2.MyTaskId);
    }

    [Fact]
    public async Task Update_DifferentUserCannotModifyTask()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var updateBody = new MyTaskUpdateDto
        {
            Title = "Hacked Title",
            Description = "Malicious update",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            Priority = TaskPriority.High,
            Status = MyTaskStatus.Completed
        };

        var updateResponse = await user2Client.PutAsJsonAsync($"/api/v1/tasks/{user1Task.MyTaskId}", updateBody, JsonOptions);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var verifyResponse = await user1Client.GetAsync($"/api/v1/tasks/{user1Task.MyTaskId}");
        var originalTask = await verifyResponse.Content.ReadFromJsonAsync<MyTaskResponseDto>(JsonOptions);
        originalTask!.Title.Should().Be("User1 Task");
        originalTask.Status.Should().Be(MyTaskStatus.NotStarted);
    }

    [Fact]
    public async Task Delete_DifferentUserCannotDeleteTask()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task to Protect");

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var deleteResponse = await user2Client.DeleteAsync($"/api/v1/tasks/{user1Task.MyTaskId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var verifyResponse = await user1Client.GetAsync($"/api/v1/tasks/{user1Task.MyTaskId}");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_TaskIsIsolatedToCurrentUser()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Secret Task");

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var user2Task = await CreateTaskViaClient(user2Client, "User1 Secret Task");

        user1Task.MyTaskId.Should().NotBe(user2Task.MyTaskId);

        var user1GetResponse = await user1Client.GetAsync($"/api/v1/tasks/{user2Task.MyTaskId}");
        user1GetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var user2GetResponse = await user2Client.GetAsync($"/api/v1/tasks/{user1Task.MyTaskId}");
        user2GetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ===== FILTERING TESTS =====

    [Fact]
    public async Task GetAll_FilterByStatus_ReturnsOnlyMatchingTasks()
    {
        var task1 = await SeedTaskAsync("Status Test 1", TaskPriority.High, 10);
        var task2 = await SeedTaskAsync("Status Test 2", TaskPriority.Medium, 10);

        var updateBody = new MyTaskUpdateDto
        {
            Title = task1.Title,
            Description = task1.Description,
            StartDate = task1.StartDate,
            EndDate = task1.EndDate,
            Priority = task1.Priority,
            Status = MyTaskStatus.InProgress
        };
        await Client.PutAsJsonAsync($"/api/v1/tasks/{task1.MyTaskId}", updateBody, JsonOptions);

        var response = await Client.GetAsync("/api/v1/tasks?status=InProgress");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);
        result!.Items.Should().Contain(t => t.MyTaskId == task1.MyTaskId && t.Status == MyTaskStatus.InProgress);
        result.Items.Should().NotContain(t => t.MyTaskId == task2.MyTaskId && t.Status == MyTaskStatus.NotStarted);
    }

    [Fact]
    public async Task GetAll_FilterByPriority_ReturnsOnlyMatchingTasks()
    {
        var taskHigh = await SeedTaskAsync("Priority High Task", TaskPriority.High);
        var taskLow = await SeedTaskAsync("Priority Low Task", TaskPriority.Low);

        var response = await Client.GetAsync("/api/v1/tasks?priority=High");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);
        result!.Items.Should().Contain(t => t.MyTaskId == taskHigh.MyTaskId && t.Priority == TaskPriority.High);
        result.Items.Should().NotContain(t => t.MyTaskId == taskLow.MyTaskId);
    }

    [Fact]
    public async Task GetAll_SearchByTitle_ReturnsMatchingTasks()
    {
        var task1 = await SeedTaskAsync("Unique Search String ABC");
        var task2 = await SeedTaskAsync("Different Title XYZ");

        var response = await Client.GetAsync("/api/v1/tasks?search=Unique%20Search");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);
        result!.Items.Should().Contain(t => t.MyTaskId == task1.MyTaskId);
        result.Items.Should().NotContain(t => t.MyTaskId == task2.MyTaskId);
    }

    [Fact]
    public async Task GetAll_SearchByDescription_ReturnsMatchingTasks()
    {
        var task1 = await SeedTaskAsync("Task 1");

        var updateBody1 = new MyTaskUpdateDto
        {
            Title = task1.Title,
            Description = "This has special keywords searchable",
            StartDate = task1.StartDate,
            EndDate = task1.EndDate,
            Priority = task1.Priority,
            Status = task1.Status
        };
        await Client.PutAsJsonAsync($"/api/v1/tasks/{task1.MyTaskId}", updateBody1, JsonOptions);

        var response = await Client.GetAsync("/api/v1/tasks?search=searchable");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);
        result!.Items.Should().Contain(t => t.MyTaskId == task1.MyTaskId);
    }

    [Fact]
    public async Task GetAll_CombineFilters_ReturnsOnlyMatching()
    {
        var taskHighPending = await SeedTaskAsync("High Priority Pending", TaskPriority.High);
        var taskHighInProgress = await SeedTaskAsync("High Priority InProgress", TaskPriority.High);
        var taskLowPending = await SeedTaskAsync("Low Priority Pending", TaskPriority.Low);

        var updateBody = new MyTaskUpdateDto
        {
            Title = taskHighInProgress.Title,
            Description = taskHighInProgress.Description,
            StartDate = taskHighInProgress.StartDate,
            EndDate = taskHighInProgress.EndDate,
            Priority = TaskPriority.High,
            Status = MyTaskStatus.InProgress
        };
        await Client.PutAsJsonAsync($"/api/v1/tasks/{taskHighInProgress.MyTaskId}", updateBody, JsonOptions);

        var response = await Client.GetAsync("/api/v1/tasks?priority=High&status=InProgress");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);
        result!.Items.Should().Contain(t => t.MyTaskId == taskHighInProgress.MyTaskId);
        result.Items.Should().NotContain(t => t.MyTaskId == taskHighPending.MyTaskId);
        result.Items.Should().NotContain(t => t.MyTaskId == taskLowPending.MyTaskId);
    }

    [Fact]
    public async Task GetAll_Pagination_ReturnsCorrectPage()
    {
        for (int i = 1; i <= 5; i++)
            await SeedTaskAsync($"Pagination Task {i}");

        var page1Response = await Client.GetAsync("/api/v1/tasks?page=1&pageSize=2");
        var page1Result = await page1Response.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);

        page1Result!.Page.Should().Be(1);
        page1Result.PageSize.Should().Be(2);
        page1Result.Items.Should().HaveCount(2);
        page1Result.TotalCount.Should().BeGreaterThanOrEqualTo(5);

        var page2Response = await Client.GetAsync("/api/v1/tasks?page=2&pageSize=2");
        var page2Result = await page2Response.Content.ReadFromJsonAsync<PagedResult<MyTaskResponseDto>>(JsonOptions);

        page2Result!.Page.Should().Be(2);
        page2Result.Items.Should().HaveCount(2);
        page1Result.Items[0].MyTaskId.Should().NotBe(page2Result.Items[0].MyTaskId);
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
}
