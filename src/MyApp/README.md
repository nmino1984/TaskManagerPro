# TaskManagerPro - Backend API

A RESTful API for task management with multi-level organization: tasks, subtasks, and milestones. Built with .NET 10 and Entity Framework Core.

## Quick Start

### Prerequisites
- **.NET 10** or higher
- **SQLite** (included with .NET)

### Setup

**Option 1: Use Example Database (Fastest)**
```bash
cd src/MyApp
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
dotnet run
```

**Option 2: Create Fresh Database**
```bash
cd src/MyApp
dotnet run
```

The application will automatically:
- Create the SQLite database via Entity Framework migrations
- Initialize all tables (Users, MyTasks, SubTasks, Milestones)
- Create performance indices
- Load seed data (Development environment only)

Access the API at `http://localhost:5141`

## Development Commands

```bash
# Start the backend server
dotnet run

# Build without running
dotnet build

# Run all tests
dotnet test

# Create a new migration
dotnet ef migrations add YourMigrationName

# Apply migrations to database
dotnet ef database update

# Reset database
dotnet ef database drop -f
dotnet ef database update
```

## API Endpoints

### Authentication
- `POST /api/v1/auth/register` - Create new user
- `POST /api/v1/auth/login` - Login user

### Tasks
- `GET /api/v1/tasks?page=1&pageSize=10` - List tasks with pagination
- `POST /api/v1/tasks` - Create task
- `PUT /api/v1/tasks/{id}` - Update task
- `DELETE /api/v1/tasks/{id}` - Delete task (soft delete)

### SubTasks
- `GET /api/v1/subtasks/bytask/{taskId}` - Get subtasks for a task
- `POST /api/v1/subtasks` - Create subtask
- `PUT /api/v1/subtasks/{id}` - Update subtask
- `DELETE /api/v1/subtasks/{id}` - Delete subtask

### Milestones
- `GET /api/v1/milestones/bytask/{taskId}` - Get milestones for a task
- `POST /api/v1/milestones` - Create milestone
- `PUT /api/v1/milestones/{id}` - Update milestone
- `DELETE /api/v1/milestones/{id}` - Delete milestone

### Milestone Export
- `GET /api/v1/milestones/bytask/{taskId}/export/json` - Export as JSON
- `GET /api/v1/milestones/bytask/{taskId}/export/xml` - Export as XML
- `GET /api/v1/milestones/bytask/{taskId}/export/ical` - Export as iCalendar (.ics)

### Notifications (Async)
- `GET /api/v1/notifications` - Get all notifications for current user
- `GET /api/v1/notifications/unread` - Get count of unread notifications
- `PATCH /api/v1/notifications/{id}/read` - Mark single notification as read
- `PATCH /api/v1/notifications/read-all` - Mark all notifications as read

**All endpoints require JWT Bearer token authentication** (except `/auth/register` and `/auth/login`)

## Background Jobs (Hangfire)

Async notifications are triggered via Hangfire background jobs:
- **Task Created**: Notification when a new task is created
- **Task Completed**: Notification when task status changes to Completed
- **Task Overdue Check**: Recurring job (hourly) to detect and notify overdue tasks

**Dashboard**: Access at `http://localhost:5141/hangfire` (Development only)

## Project Structure

```
src/MyApp/
├── Domain/                    # Core entities
│   ├── Entities/
│   └── Enums/
├── Application/               # Business logic
│   ├── Services/
│   ├── Interfaces/
│   ├── DTOs/
│   └── Validators/
├── Infrastructure/            # Data access layer
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── DataSeeder.cs
│   └── Migrations/
├── API/                       # Controllers & middleware
│   ├── Controllers/
│   ├── Middleware/
│   └── Validators/
├── Program.cs                 # Application startup
└── appsettings*.json          # Configuration
```

## Architecture Principles

- **Clean Architecture**: Separation of concerns (Domain → Application → Infrastructure → API)
- **Dependency Injection**: All services registered in Program.cs
- **DTOs**: Request/response objects for API contracts
- **AutoMapper**: DTO mapping to domain entities
- **Fluent Validation**: Server-side validation rules
- **Entity Framework Core**: ORM for database operations
- **Soft Delete**: Logical deletion with `IsDeleted` flag

## Authentication & Security

- **JWT Bearer Tokens**: Secure token-based authentication
- **Password Hashing**: BCrypt for secure password storage
- **User Isolation**: Multi-tenant architecture - users only see their own data
- **Authorization**: All operations validate user ownership
- **CORS**: Configured for frontend origin

### Environment Variables

Required for production:
```bash
JWT_KEY="your-256-bit-secret-key"
ASPNETCORE_ENVIRONMENT="Production"
```

Development uses hardcoded defaults in `appsettings.Development.json`.

## Database

The database uses SQLite for development with Entity Framework Core migrations.

### Database Location
```
src/MyApp/TaskManagerPro.db
src/MyApp/TaskManagerPro.db-shm  # WAL shared memory file
src/MyApp/TaskManagerPro.db-wal  # Write-ahead log
```

These files are Git-ignored and created locally when the application runs.

### Schema Management

Never edit the database schema manually. Use Entity Framework migrations:

```bash
# View existing migrations
ls src/MyApp/Infrastructure/Migrations/

# Create new migration after modifying entities
dotnet ef migrations add DescriptiveName

# Apply migrations
dotnet ef database update
```

## Testing

### Run Integration Tests
```bash
dotnet test
```

Expected: **22/22 tests passing**

### Test Architecture
- **Framework**: xUnit
- **Assertions**: FluentAssertions
- **Database**: In-memory SQLite (`:memory:`)
- **Factory**: CustomWebApplicationFactory for test setup

Tests use an isolated in-memory database - no cleanup needed.

## Data Seeding

In Development environment, the `DataSeeder` class automatically populates the database with sample data:
- Demo tasks with various priorities and statuses
- Subtasks and milestones
- Sample data for testing

To modify seed data, edit `src/MyApp/Infrastructure/Data/DataSeeder.cs` and restart.

## Configuration

### Development (`appsettings.Development.json`)
- SQLite local file database
- Auto-migrations enabled
- Auto-seeding enabled
- Development logging level
- Hardcoded JWT key

### Production
- Set `JWT_KEY` environment variable
- Configure production database connection string
- Set `ASPNETCORE_ENVIRONMENT=Production`

## Troubleshooting

### Port Already in Use
```bash
# Windows
netstat -ano | findstr :5141

# macOS/Linux
lsof -i :5141
```

### Database Locked
```bash
# Remove WAL files and restart
rm src/MyApp/TaskManagerPro.db-shm src/MyApp/TaskManagerPro.db-wal
dotnet run
```

### Migrations Out of Sync
```bash
dotnet ef database drop -f
dotnet ef database update
dotnet run
```

### Clean Build
```bash
dotnet clean
dotnet build
dotnet run
```

## Dependencies

Key NuGet packages:
- **Microsoft.EntityFrameworkCore**: ORM framework
- **Microsoft.IdentityModel.Tokens**: JWT token handling
- **BCrypt.Net**: Password hashing
- **FluentValidation**: Server-side validation
- **Ical.Net**: iCalendar export format
- **Serilog**: Structured logging
- **AutoMapper**: Object mapping
- **Hangfire.Core**: Background job scheduling
- **Hangfire.AspNetCore**: ASP.NET Core integration
- **Hangfire.MemoryStorage**: In-memory job storage (development)
- **Hangfire.SqlServer**: SQL Server job storage (production)
- **xUnit**: Testing framework
- **FluentAssertions**: Test assertions

## Related Documentation

- **Frontend Setup**: See `frontend/README.md`
- **Database Management**: See `DATABASE_SETUP.md` in project root
- **Full Project Overview**: See `README.md` in project root

## Support

For database questions, see `DATABASE_SETUP.md` for detailed setup, data loading, and cleanup procedures.
