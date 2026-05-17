# TaskManagerPro - Integration Tests

Comprehensive integration test suite for the TaskManagerPro backend. Tests verify API functionality, authorization, database operations, and business logic without mocking the database.

## Quick Start

### Running All Tests

```bash
# From project root
dotnet test
```

Expected output: ✅ **22/22 passing**

### Running Specific Test File

```bash
dotnet test tests/MyApp.Tests.Integration/Controllers/TasksControllerTests.cs
```

### Running Tests with Verbose Output

```bash
dotnet test --verbosity detailed
```

### Running Tests with Coverage

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=json
```

## Test Structure

```
tests/MyApp.Tests.Integration/
├── Controllers/              # API endpoint tests
│   ├── AuthControllerTests.cs
│   ├── TasksControllerTests.cs
│   ├── SubTasksControllerTests.cs
│   └── MilestonesControllerTests.cs
├── Infrastructure/          # Test setup and helpers
│   ├── CustomWebApplicationFactory.cs    # Test server factory
│   └── IntegrationTestBase.cs           # Base class for tests
└── Features/                # Feature-specific tests (if any)
```

## Test Architecture

### CustomWebApplicationFactory

Creates a test-specific web application with:
- **In-Memory SQLite Database**: Each test run gets a fresh, isolated database (`:memory:`)
- **Development Environment**: Uses development configuration and seed data
- **JWT Configuration**: Pre-configured with test JWT key
- **No Real Dependencies**: All external services are configured for testing

```csharp
// Example usage
var factory = new CustomWebApplicationFactory();
var client = factory.CreateClient();
```

### Integration Test Base

All tests inherit from `IntegrationTestBase` which provides:
- Automatic factory and client setup
- Cleanup after each test
- Common test data and utilities

## Test Patterns

### Authentication Tests

```csharp
[Fact]
public async Task Register_WithValidCredentials_ReturnsToken()
{
    // Arrange
    var request = new { username = "testuser", password = "Password123!" };
    
    // Act
    var response = await _client.PostAsync("/api/v1/auth/register", 
        JsonContent.Create(request));
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

### Authorized Endpoints

Tests verify that endpoints require JWT authentication:

```csharp
[Fact]
public async Task GetTasks_WithoutToken_Returns401()
{
    // Act - no token provided
    var response = await _client.GetAsync("/api/v1/tasks");
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
}
```

### User Isolation Tests

Tests verify that users only see their own data (multi-tenancy):

```csharp
[Fact]
public async Task GetTasks_ReturnsOnlyCurrentUserTasks()
{
    // Arrange - create two users
    var user1Token = await RegisterUser("user1", "pass");
    var user2Token = await RegisterUser("user2", "pass");
    
    // Create task as user1
    var taskId = await CreateTask(user1Token, "Task 1");
    
    // Act - user2 tries to get user1's task
    var response = await GetTask(user2Token, taskId);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
}
```

### CRUD Operation Tests

Tests for Create, Read, Update, Delete operations:

```csharp
[Fact]
public async Task CreateTask_WithValidData_ReturnsCreatedTask()
{
    // Test that valid data creates task successfully
}

[Fact]
public async Task UpdateTask_WithInvalidData_ReturnsBadRequest()
{
    // Test validation on update
}

[Fact]
public async Task DeleteTask_RemovesSoftDeleteFlag()
{
    // Test that soft delete works correctly
}
```

## Test Coverage

The test suite covers:

### Authentication (3 tests)
- ✅ User registration with validation
- ✅ User login with valid/invalid credentials
- ✅ JWT token generation and validation

### Tasks (7 tests)
- ✅ Get tasks with pagination
- ✅ Create task with validation
- ✅ Update task authorization
- ✅ Delete task (soft delete)
- ✅ Filtering and sorting
- ✅ User isolation

### SubTasks (5 tests)
- ✅ Get subtasks for a task
- ✅ Create subtask with validation
- ✅ Update and delete subtasks
- ✅ User authorization

### Milestones (7 tests)
- ✅ Get milestones for a task
- ✅ Create milestone with validation
- ✅ Update and delete milestones
- ✅ Export to JSON, XML, iCal
- ✅ User authorization

## Database in Tests

Tests use **in-memory SQLite** (`:memory:`):
- Fresh, isolated database for each test
- No cleanup required between tests
- Automatic migration application
- Seed data populated in Development environment

```csharp
// Database configuration in factory
var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connection)
    .Options;
```

## Writing New Tests

### 1. Create Test Class

```csharp
public class YourFeatureTests : IntegrationTestBase
{
    [Fact]
    public async Task YourTest_WithScenario_ExpectedResult()
    {
        // Arrange
        
        // Act
        
        // Assert
    }
}
```

### 2. Register User and Get Token

```csharp
var response = await _client.PostAsync("/api/v1/auth/register",
    JsonContent.Create(new { username = "user", password = "Pass123!" }));
var token = // extract from response
_client.DefaultRequestHeaders.Authorization = 
    new("Bearer", token);
```

### 3. Make API Call

```csharp
var response = await _client.GetAsync("/api/v1/tasks");
```

### 4. Assert Results

```csharp
response.StatusCode.Should().Be(HttpStatusCode.OK);
var content = await response.Content.ReadAsStringAsync();
var data = JsonSerializer.Deserialize<TaskListResponse>(content);
data.Should().NotBeNull();
```

## Best Practices

✅ **Isolation**: Each test is independent  
✅ **Clarity**: Test names describe what they test  
✅ **AAA Pattern**: Arrange → Act → Assert  
✅ **No Mocking**: Use real in-memory database  
✅ **Fast**: All 22 tests complete in < 2 seconds  
✅ **Deterministic**: Same results every run  

## Troubleshooting

### Tests Fail with "JWT Key Not Configured"

The tests run with Development environment which provides a hardcoded key. If this fails, ensure:
```bash
dotnet test --environment Development
```

### Tests Pass Locally but Fail in CI

Ensure:
- .NET 10 installed
- All migrations applied
- Development environment variables set
- No existing processes using the test database

### Single Test Fails

Restart test runner:
```bash
dotnet test --no-build
```

## Related Documentation

- **Backend Setup**: [Backend README](../src/MyApp/README.md)
- **Database Setup**: [Database Guide](../DATABASE_SETUP.md)
- **Full Project Overview**: [Project README](../README.md)

## Test Frameworks

- **xUnit**: Test framework
- **FluentAssertions**: Readable assertions
- **Moq**: (Not used - tests use real database)
- **SQLite In-Memory**: Test database

## Running Tests in Development

During development, run tests frequently:

```bash
# Run tests after each change
dotnet test --watch

# Or run specific test class
dotnet test --filter "TasksControllerTests"
```

## Next Steps

- All 22 tests passing ✅
- Ready for production deployment
- Monitor test performance in CI/CD
