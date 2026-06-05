using MyApp.Application.DTOs.Auth;
using MyApp.Application.DTOs.Milestone;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.DTOs.SubTask;
using MyApp.Domain.Enums;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyApp.Tests.Integration.Infrastructure;

public abstract class IntegrationTestBase
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;
    protected string? Token;

    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task AuthenticateAsync(string? username = null, string password = "TestPassword123!")
    {
        username ??= $"testuser_{Guid.NewGuid():N}";

        var registerBody = new RegisterDto { Username = username, Password = password };
        var registerResponse = await Client.PostAsJsonAsync("/api/v1/auth/register", registerBody, JsonOptions);
        registerResponse.EnsureSuccessStatusCode();

        var authResult = (await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions))!;
        Token = authResult.Token;

        Client.DefaultRequestHeaders.Remove("Authorization");
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
    }

    protected async Task<(HttpClient client, string token, string username)> CreateAuthenticatedClientAsync(
        string? username = null,
        string password = "TestPassword123!")
    {
        username ??= $"testuser_{Guid.NewGuid():N}";

        var client = Factory.CreateClient();

        var registerBody = new RegisterDto { Username = username, Password = password };
        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", registerBody, JsonOptions);
        registerResponse.EnsureSuccessStatusCode();

        var authResult = (await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions))!;
        var token = authResult.Token;

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        return (client, token, username);
    }

    protected async Task<MyTaskResponseDto> SeedTaskAsync(
        string title = "Seed Task",
        TaskPriority priority = TaskPriority.Medium,
        int daysUntilEnd = 30)
    {
        if (Token == null)
        {
            await AuthenticateAsync();
        }

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
        if (Token == null)
        {
            await AuthenticateAsync();
        }

        var body = new SubTaskCreateDto { TaskId = taskId, Description = description };
        var response = await Client.PostAsJsonAsync("/api/v1/subtasks", body, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SubTaskResponseDto>(JsonOptions))!;
    }

    protected async Task<MilestoneResponseDto> SeedMilestoneAsync(
        int taskId,
        string title = "Seed Milestone",
        int daysUntilTarget = 30)
    {
        if (Token == null)
        {
            await AuthenticateAsync();
        }

        var body = new MilestoneCreateDto
        {
            TaskId = taskId,
            Title = title,
            Description = "Seeded for integration testing",
            TargetDate = DateTime.UtcNow.AddDays(daysUntilTarget)
        };
        var response = await Client.PostAsJsonAsync("/api/v1/milestones", body, JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<MilestoneResponseDto>(JsonOptions))!;
    }

}
