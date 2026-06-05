# TaskManagerPro

A full-stack task management application built with **.NET 10** and **Angular 21**. Supports multi-user task organization with subtasks, milestones, comments, audit history, and async background notifications.

---

## Features

- **Tasks** — CRUD with priority levels, status tracking, pagination, search, and filtering
- **Subtasks** — Break work down; task progress auto-calculates from subtask completion
- **Milestones** — Key checkpoints with export to JSON, XML, or iCalendar (.ics)
- **Task Assignment** — Assign tasks to other registered users
- **Comments** — Per-task comments with @mention notifications
- **Audit Trail** — Full history of changes to tasks, subtasks, and milestones
- **Notifications** — In-app notifications via Hangfire background jobs; unread badge in navbar
- **Authentication** — JWT with BCrypt password hashing; all data is user-scoped (multi-tenancy)

---

## Quick Start

**Prerequisites:** .NET 10, Node.js 18+

```bash
# Option 1: Use the included example database (fastest)
cd src/TaskManagerPro.API
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
dotnet run

# Option 2: Fresh database (auto-created on first run)
cd src/TaskManagerPro.API && dotnet run
```

```bash
# Frontend (in a separate terminal)
cd frontend
npm install
npm start
```

Open `http://localhost:4200` — register an account and start creating tasks.

---

## Project Structure

```
TaskManagerPro/
├── src/
│   ├── TaskManagerPro.Domain/          # Entities, enums — zero external dependencies
│   ├── TaskManagerPro.Application/     # Services, DTOs, interfaces, validators
│   ├── TaskManagerPro.Infrastructure/  # EF Core, repositories, Hangfire, JWT, BCrypt
│   └── TaskManagerPro.API/             # Controllers, middleware, Program.cs
├── tests/
│   ├── TaskManagerPro.Integration.Tests/   # 64 integration tests (real SQLite in-memory)
│   └── TaskManagerPro.Unit.Tests/          # 11 unit tests (domain logic)
├── frontend/                           # Angular 21 app (standalone components, signals)
├── TaskManagerPro.db.example           # Example database for quick start
└── DATABASE_SETUP.md                   # Database setup and management guide
```

**Architecture:** Clean Architecture — layers point inward toward Domain. Application never references Infrastructure.

---

## API Endpoints

All endpoints require `Authorization: Bearer {token}` except auth.

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/auth/register` | Register new user |
| POST | `/api/v1/auth/login` | Login |
| GET | `/api/v1/tasks` | List tasks (pagination, filter, search) |
| POST | `/api/v1/tasks` | Create task |
| GET | `/api/v1/tasks/{id}` | Get task |
| PUT | `/api/v1/tasks/{id}` | Update task |
| DELETE | `/api/v1/tasks/{id}` | Soft-delete task |
| PATCH | `/api/v1/tasks/{id}/assign` | Assign task to another user |
| GET | `/api/v1/tasks/{id}/history` | Task audit history |
| GET | `/api/v1/subtasks/bytask/{taskId}` | List subtasks |
| POST | `/api/v1/subtasks` | Create subtask |
| PUT | `/api/v1/subtasks/{id}` | Update subtask |
| DELETE | `/api/v1/subtasks/{id}` | Delete subtask |
| GET | `/api/v1/subtasks/{id}/history` | Subtask audit history |
| GET | `/api/v1/milestones/bytask/{taskId}` | List milestones |
| POST | `/api/v1/milestones` | Create milestone |
| PUT | `/api/v1/milestones/{id}` | Update milestone |
| DELETE | `/api/v1/milestones/{id}` | Delete milestone |
| GET | `/api/v1/milestones/bytask/{taskId}/export/json` | Export milestones as JSON |
| GET | `/api/v1/milestones/bytask/{taskId}/export/xml` | Export milestones as XML |
| GET | `/api/v1/milestones/bytask/{taskId}/export/ical` | Export milestones as iCal |
| GET | `/api/v1/milestones/{id}/history` | Milestone audit history |
| GET | `/api/v1/tasks/{taskId}/comments` | List comments |
| POST | `/api/v1/tasks/{taskId}/comments` | Create comment |
| PUT | `/api/v1/comments/{id}` | Edit comment |
| DELETE | `/api/v1/comments/{id}` | Delete comment |
| GET | `/api/v1/notifications` | List notifications |
| GET | `/api/v1/notifications/unread` | Unread count |
| PATCH | `/api/v1/notifications/{id}/read` | Mark one as read |
| PATCH | `/api/v1/notifications/read-all` | Mark all as read |

---

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Backend | .NET 10, ASP.NET Core, Entity Framework Core |
| Frontend | Angular 21, Angular Material, TypeScript, Signals |
| Database | SQLite (development), SQL Server (production) |
| Background Jobs | Hangfire with MemoryStorage (dev) / SqlServer (prod) |
| Auth | JWT Bearer, BCrypt |
| Validation | FluentValidation (backend), Angular Reactive Forms (frontend) |
| Testing | xUnit, FluentAssertions, in-memory SQLite |

---

## Running Tests

```bash
# All 75 tests
dotnet test TaskManagerPro.sln

# Specific suite
dotnet test --filter "TasksControllerTests"
```

Expected: **75 passing** (11 unit + 64 integration)

---

## Configuration

**Development** — defaults in `src/TaskManagerPro.API/appsettings.Development.json`. No setup needed.

**Production** — set environment variables:
```bash
JWT_KEY=your-256-bit-secret-key
ConnectionStrings__DefaultConnection=Server=...;Database=TaskManagerPro;...
ASPNETCORE_ENVIRONMENT=Production
```

CORS origins are configured in `appsettings.Development.json` under `"CorsOrigins"`. If Angular starts on a different port, add it there.

Hangfire dashboard: `http://localhost:5141/hangfire` (development only)

---

## Database

See [DATABASE_SETUP.md](DATABASE_SETUP.md) for initialization, seeding, migrations, and reset instructions.

---

## License

Provided as-is for educational and demonstration purposes.
