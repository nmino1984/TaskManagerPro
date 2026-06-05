using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerPro.Application.DTOs.Milestone;
using TaskManagerPro.Application.DTOs.MyTask;
using TaskManagerPro.Domain.Enums;
using TaskManagerPro.Integration.Tests.Infrastructure;
using Xunit;

namespace TaskManagerPro.Integration.Tests.Controllers;

public class MilestonesControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
{
    public MilestonesControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    // ===== BASIC CRUD TESTS =====

    [Fact]
    public async Task GetByTask_Returns200WithMilestones()
    {
        var task = await SeedTaskAsync();
        await SeedMilestoneAsync(task.MyTaskId, "Milestone 1");
        await SeedMilestoneAsync(task.MyTaskId, "Milestone 2");

        var response = await Client.GetAsync($"/api/v1/milestones/bytask/{task.MyTaskId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var milestones = await response.Content.ReadFromJsonAsync<List<MilestoneResponseDto>>(JsonOptions);
        milestones!.Should().HaveCountGreaterOrEqualTo(2);
        milestones.Should().Contain(m => m.Title == "Milestone 1");
        milestones.Should().Contain(m => m.Title == "Milestone 2");
    }

    [Fact]
    public async Task GetById_ExistingId_Returns200()
    {
        var task = await SeedTaskAsync();
        var seeded = await SeedMilestoneAsync(task.MyTaskId, "Specific Milestone");

        var response = await Client.GetAsync($"/api/v1/milestones/{seeded.MilestoneId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var milestone = await response.Content.ReadFromJsonAsync<MilestoneResponseDto>(JsonOptions);
        milestone!.MilestoneId.Should().Be(seeded.MilestoneId);
        milestone.Title.Should().Be("Specific Milestone");
    }

    [Fact]
    public async Task GetById_NonExistingId_Returns404()
    {
        await AuthenticateAsync();
        var response = await Client.GetAsync("/api/v1/milestones/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_ValidDto_Returns201WithDefaultPendingStatus()
    {
        var task = await SeedTaskAsync();
        var body = new MilestoneCreateDto
        {
            TaskId = task.MyTaskId,
            Title = "Launch Phase",
            Description = "Go live milestone",
            TargetDate = DateTime.UtcNow.AddDays(30)
        };

        var response = await Client.PostAsJsonAsync("/api/v1/milestones", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<MilestoneResponseDto>(JsonOptions);
        created!.TaskId.Should().Be(task.MyTaskId);
        created.Title.Should().Be("Launch Phase");
        created.Status.Should().Be(MilestoneStatus.Pending);
    }

    [Fact]
    public async Task Create_NonExistingTaskId_Returns404()
    {
        await AuthenticateAsync();
        var body = new MilestoneCreateDto
        {
            TaskId = 99999,
            Title = "Orphan Milestone",
            Description = "No task",
            TargetDate = DateTime.UtcNow.AddDays(30)
        };

        var response = await Client.PostAsJsonAsync("/api/v1/milestones", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_EmptyTitle_Returns400()
    {
        var task = await SeedTaskAsync();
        var body = new MilestoneCreateDto
        {
            TaskId = task.MyTaskId,
            Title = "",
            Description = "Empty title",
            TargetDate = DateTime.UtcNow.AddDays(30)
        };

        var response = await Client.PostAsJsonAsync("/api/v1/milestones", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Title");
    }

    [Fact]
    public async Task Create_CompletedMilestone_AffectsTaskTracking()
    {
        var task = await SeedTaskAsync("Milestone Tracking Task");
        var body = new MilestoneCreateDto
        {
            TaskId = task.MyTaskId,
            Title = "Critical Milestone",
            Description = "Important checkpoint",
            TargetDate = DateTime.UtcNow.AddDays(7),
            Status = MilestoneStatus.Completed
        };

        await Client.PostAsJsonAsync("/api/v1/milestones", body, JsonOptions);

        var taskResponse = await Client.GetAsync($"/api/v1/tasks/{task.MyTaskId}");
        var updatedTask = await taskResponse.Content.ReadFromJsonAsync<MyTaskResponseDto>(JsonOptions);
        updatedTask!.MyTaskId.Should().Be(task.MyTaskId);

        var milestonesResponse = await Client.GetAsync($"/api/v1/milestones/bytask/{task.MyTaskId}");
        var milestones = await milestonesResponse.Content.ReadFromJsonAsync<List<MilestoneResponseDto>>(JsonOptions);
        milestones!.Should().Contain(m => m.Title == "Critical Milestone" && m.Status == MilestoneStatus.Completed);
    }

    [Fact]
    public async Task Update_ExistingId_Returns200WithNewValues()
    {
        var task = await SeedTaskAsync();
        var seeded = await SeedMilestoneAsync(task.MyTaskId, "Before Update");
        var body = new MilestoneUpdateDto
        {
            Title = "After Update",
            Description = "Updated description",
            TargetDate = DateTime.UtcNow.AddDays(45),
            Status = MilestoneStatus.Completed
        };

        var response = await Client.PutAsJsonAsync($"/api/v1/milestones/{seeded.MilestoneId}", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<MilestoneResponseDto>(JsonOptions);
        updated!.Title.Should().Be("After Update");
        updated.Status.Should().Be(MilestoneStatus.Completed);
    }

    [Fact]
    public async Task Update_NonExistingId_Returns404()
    {
        await AuthenticateAsync();
        var body = new MilestoneUpdateDto
        {
            Title = "Ghost",
            Description = "Not real",
            TargetDate = DateTime.UtcNow.AddDays(30),
            Status = MilestoneStatus.Pending
        };

        var response = await Client.PutAsJsonAsync("/api/v1/milestones/99999", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ExistingId_Returns204()
    {
        var task = await SeedTaskAsync();
        var seeded = await SeedMilestoneAsync(task.MyTaskId, "To Delete");

        var deleteResponse = await Client.DeleteAsync($"/api/v1/milestones/{seeded.MilestoneId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/v1/milestones/{seeded.MilestoneId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NonExistingId_Returns404()
    {
        await AuthenticateAsync();
        var response = await Client.DeleteAsync("/api/v1/milestones/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ===== AUTHENTICATION TESTS =====

    [Fact]
    public async Task GetByTask_WithoutAuthorizationHeader_Returns401Unauthorized()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/v1/milestones/bytask/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithoutAuthorizationHeader_Returns401Unauthorized()
    {
        var client = Factory.CreateClient();
        var body = new MilestoneCreateDto
        {
            TaskId = 1,
            Title = "Unauthorized",
            Description = "No token",
            TargetDate = DateTime.UtcNow.AddDays(30)
        };

        var response = await client.PostAsJsonAsync("/api/v1/milestones", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ===== MULTI-TENANCY TESTS =====

    [Fact]
    public async Task GetByTask_DifferentUserCannotAccessOthersMilestones()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");
        await CreateMilestoneViaClient(user1Client, user1Task.MyTaskId, "User1 Milestone");

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var response = await user2Client.GetAsync($"/api/v1/milestones/bytask/{user1Task.MyTaskId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_DifferentUserCannotAccessOthersMilestone()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");
        var user1Milestone = await CreateMilestoneViaClient(user1Client, user1Task.MyTaskId, "User1 Milestone");

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var response = await user2Client.GetAsync($"/api/v1/milestones/{user1Milestone.MilestoneId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_CannotCreateMilestoneForOthersTask()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var body = new MilestoneCreateDto
        {
            TaskId = user1Task.MyTaskId,
            Title = "Hacked Milestone",
            Description = "Unauthorized",
            TargetDate = DateTime.UtcNow.AddDays(30)
        };

        var response = await user2Client.PostAsJsonAsync("/api/v1/milestones", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DifferentUserCannotModifyOthersMilestone()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");
        var user1Milestone = await CreateMilestoneViaClient(user1Client, user1Task.MyTaskId, "Original Milestone");

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var updateBody = new MilestoneUpdateDto
        {
            Title = "Hacked Milestone",
            Description = "Malicious edit",
            TargetDate = DateTime.UtcNow.AddDays(60),
            Status = MilestoneStatus.Completed
        };

        var response = await user2Client.PutAsJsonAsync($"/api/v1/milestones/{user1Milestone.MilestoneId}", updateBody, JsonOptions);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var verifyResponse = await user1Client.GetAsync($"/api/v1/milestones/{user1Milestone.MilestoneId}");
        var original = await verifyResponse.Content.ReadFromJsonAsync<MilestoneResponseDto>(JsonOptions);
        original!.Title.Should().Be("Original Milestone");
        original.Status.Should().Be(MilestoneStatus.Pending);
    }

    [Fact]
    public async Task Delete_DifferentUserCannotDeleteOthersMilestone()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");
        var user1Milestone = await CreateMilestoneViaClient(user1Client, user1Task.MyTaskId, "Protected Milestone");

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var deleteResponse = await user2Client.DeleteAsync($"/api/v1/milestones/{user1Milestone.MilestoneId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var verifyResponse = await user1Client.GetAsync($"/api/v1/milestones/{user1Milestone.MilestoneId}");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ===== EXPORT TESTS =====

    [Fact]
    public async Task ExportJson_Returns200WithJsonContent()
    {
        var task = await SeedTaskAsync();
        await SeedMilestoneAsync(task.MyTaskId, "Export Milestone");

        var response = await Client.GetAsync($"/api/v1/milestones/bytask/{task.MyTaskId}/export/json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Export Milestone");
    }

    [Fact]
    public async Task ExportXml_Returns200WithXmlContent()
    {
        var task = await SeedTaskAsync();
        await SeedMilestoneAsync(task.MyTaskId, "XML Milestone");

        var response = await Client.GetAsync($"/api/v1/milestones/bytask/{task.MyTaskId}/export/xml");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/xml");
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("XML Milestone");
    }

    [Fact]
    public async Task ExportIcal_Returns200WithIcalContent()
    {
        var task = await SeedTaskAsync();
        await SeedMilestoneAsync(task.MyTaskId, "Calendar Milestone");

        var response = await Client.GetAsync($"/api/v1/milestones/bytask/{task.MyTaskId}/export/ical");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/calendar");
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("BEGIN:VCALENDAR");
    }

    [Fact]
    public async Task ExportJson_DifferentUserCannotExportOthersData()
    {
        var (user1Client, _, _) = await CreateAuthenticatedClientAsync();
        var user1Task = await CreateTaskViaClient(user1Client, "User1 Task");
        await CreateMilestoneViaClient(user1Client, user1Task.MyTaskId, "Secret Milestone");

        var (user2Client, _, _) = await CreateAuthenticatedClientAsync();
        var response = await user2Client.GetAsync($"/api/v1/milestones/bytask/{user1Task.MyTaskId}/export/json");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ===== HELPER METHODS =====

    private async Task<MyTaskResponseDto> CreateTaskViaClient(HttpClient client, string title)
    {
        var body = new MyTaskCreateDto
        {
            Title = title,
            Description = "Test task",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30),
            Priority = TaskPriority.Medium
        };
        var response = await client.PostAsJsonAsync("/api/v1/tasks", body, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MyTaskResponseDto>(JsonOptions))!;
    }

    private async Task<MilestoneResponseDto> CreateMilestoneViaClient(HttpClient client, int taskId, string title)
    {
        var body = new MilestoneCreateDto
        {
            TaskId = taskId,
            Title = title,
            Description = "Test milestone",
            TargetDate = DateTime.UtcNow.AddDays(30)
        };
        var response = await client.PostAsJsonAsync("/api/v1/milestones", body, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MilestoneResponseDto>(JsonOptions))!;
    }
}
