# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 🔄 Current Status (2026-05-24)

**Phase:** Bug Fixes + Feature Foundation Complete ✅

### Recent Fixes (Today's Work)
1. **SubTask Creation Error** - FIXED
   - Issue: Frontend was sending `undefined` taskId, causing 400 errors
   - Solution: Added validation in SubTaskFormComponent constructor and onSubmit
   - Frontend now validates taskId before API call

2. **Enum Serialization Error** - FIXED
   - Issue: Frontend sending "Pending" (PascalCase), backend expecting "pending" (camelCase)
   - Root cause: JsonStringEnumConverter(JsonNamingPolicy.CamelCase) in Program.cs
   - Solution: Frontend converts status to lowercase before sending

3. **Date Validation Error** - FIXED
   - Issue: SubTask validator rejected any date ≤ DateTime.UtcNow
   - Problem: Validation logic made no sense for educational app
   - Solution: Removed date validation completely (user can set any date)

### Frontend Components Working
- ✅ Task List (with pagination, filters, search)
- ✅ Task Form (Create + Edit modes)
- ✅ SubTask List (with CRUD operations)
- ✅ SubTask Form (Create + Edit with proper validation)
- ✅ Milestone List and Form
- ✅ Authentication (Login/Register)
- ✅ Notifications with Hangfire background jobs

### Next Phase
Ready to implement one of:
- **Task Assignment** (assign tasks to other users)
- **Task History/Audit** (track all changes)
- **Task Comments** (collaborative feature)

---

## Quick Commands

### Backend (.NET 10)
```bash
cd src/MyApp

# Run development server (creates/migrates DB automatically)
dotnet run

# Build and verify no errors
dotnet build

# Run all 51+ integration tests
dotnet test

# Run specific test class
dotnet test --filter "TasksControllerTests"

# Create new migration after modifying entities
dotnet ef migrations add DescriptiveName

# Apply pending migrations to database
dotnet ef database update

# Reset database to fresh state
dotnet ef database drop -f && dotnet ef database update
```

### Frontend (Angular 21)
```bash
cd frontend

# Install dependencies
npm install

# Start development server (http://localhost:4200)
npm start

# Production build
npm run build

# Lint and format
npm run lint
npm run format
```

### Database Setup
```bash
# Use example database (fastest)
cd src/MyApp
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
dotnet run

# Or let it auto-create on first run
cd src/MyApp && dotnet run
```

## Architecture Overview

**Clean Architecture (layers point inward toward Domain):**
```
API (Controllers)
  ↓ depends on
Application (Services, DTOs, Interfaces)
  ↓ depends on
Infrastructure (EF Core, DbContext) + Domain (Entities, Enums)
```

### Domain (Core Business Logic)
- **Location:** `src/MyApp/Domain/`
- Pure entities with business methods: `MyTask.UpdateProgress()`, `MyTask.UpdateStatus()`
- Enums for status/priority (TaskPriority, MyTaskStatus, SubTaskStatus, MilestoneStatus)
- Zero external dependencies

### Application (Orchestration & Validation)
- **Location:** `src/MyApp/Application/`
- Services: `TaskService`, `AuthService`, `SubTaskService`, `MilestoneService`
- All implement interfaces in `Application/Interfaces/`
- DTOs for request/response contracts
- FluentValidation rules in `Validators/`
- **Key pattern:** Services depend on `ITaskService` (interface), never concrete implementation

### Infrastructure (Persistence)
- **Location:** `src/MyApp/Infrastructure/`
- `AppDbContext`: FluentAPI configuration (no Data Annotations)
  - QueryFilter for soft delete: `HasQueryFilter(t => !t.IsDeleted)` — automatically filters deleted records
  - Composite index on (UserId, IsDeleted, CreatedAt) for main task queries
  - EnumToStringConverter for storing enums as strings, not numbers
  - Cascade delete: `OnDelete(DeleteBehavior.Cascade)`
- `DataSeeder`: Static class that seeds demo data (only in Development)
- Migrations managed via EF Core migrations

### API (HTTP Layer)
- **Location:** `src/MyApp/API/Controllers/`
- Controllers depend on `ITaskService` interface (Application layer)
- Extract `UserId` from JWT claim: `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`
- ValidationFilter handles FluentValidation errors automatically
- All endpoints require [Authorize] attribute

## Multi-Tenancy & Security

**User Isolation Pattern:**
- Every service method receives `userId` parameter extracted from JWT claim
- Services filter queries by `t.UserId == userId` before any operation
- If operation not found for user, throws `NotFoundException` (returns 404, hiding existence)
- **Test Coverage:** 51+ tests validate that User A cannot access/modify/delete User B's data

**JWT Configuration:**
- Token generation uses `GetJwtKey()` method
- Production: reads from environment variable `JWT_KEY`
- Development: uses hardcoded key fallback

## Testing Architecture

**51+ Integration Tests (all 4 controllers)**
```
tests/MyApp.Tests.Integration/Controllers/
├── TasksControllerTests.cs (19 tests: CRUD + multi-tenancy + filtering + pagination)
├── SubTasksControllerTests.cs (14 tests: CRUD + multi-tenancy)
├── MilestonesControllerTests.cs (21+ tests: CRUD + multi-tenancy + export + progress)
└── AuthControllerTests.cs (implied in auth tests)
```

**Key Test Pattern:**
- `CustomWebApplicationFactory`: Creates fresh SQLite `:memory:` database per factory instance
- `IntegrationTestBase`: Base class with helpers
- **Critical Helper:** `CreateAuthenticatedClientAsync()` — creates independent HttpClient with unique user/token for multi-tenant testing
- All tests use real in-memory SQLite, no mocking of data layer
- Each test gets isolated database (cascade delete on user → auto-cleanup)

**To Add Tests:**
1. Multi-tenancy: Use `CreateAuthenticatedClientAsync()` to create User 1 and User 2 with independent clients
2. Filtering: Create multiple items with different statuses/priorities, query with filter params
3. Pagination: Create 5+ items, test different page/pageSize combinations

## Database Specifics

**Development (SQLite):**
- File: `src/MyApp/TaskManagerPro.db` (auto-created on first run)
- Soft delete: Tasks marked `IsDeleted = true` are hidden by QueryFilter
- QueryFilter ensures no query accidentally returns deleted records

**Production (SQL Server):**
- Configure connection string in environment or appsettings.Production.json
- Run `dotnet ef database update` to apply migrations
- Same EF Core code works without changes (abstraction via DbContext)

**Switching Databases:**
- Only need to change Program.cs line with `.UseSqlite()` or `.UseNpgsql()` or `.UseSqlServer()`
- All service code stays unchanged (EF Core abstraction)

## Important Architectural Decisions

1. **FluentAPI only** (no Data Annotations) — keeps entities clean, all mapping logic in one place
2. **QueryFilter for soft delete** — no forgetting `.Where(!IsDeleted)` in queries
3. **CreateAuthenticatedClientAsync() for tests** — each user gets isolated client/token, makes multi-tenant testing straightforward
4. **Interface-based services** — Program.cs registers `ITaskService → TaskService`, controllers depend on interface
5. **DataSeeder is static** — runs once at startup, not via DI (acceptable tradeoff for simplicity)

## Frontend-Backend Contract

**API Base URL:**
- Development: `http://localhost:5141/api/v1`
- Production: `/api/v1` (relative path)

**Authentication:**
- POST `/api/v1/auth/register` → returns `{ token, userId, username }`
- POST `/api/v1/auth/login` → returns same
- Frontend stores `userId` and `token`, includes `Authorization: Bearer {token}` in all requests

**Multi-Tenancy:**
- Backend extracts `userId` from JWT claim, validates ownership before returning/modifying data
- Frontend doesn't need to enforce — backend rejects with 404 if user doesn't own resource

## When Making Changes

1. **Adding a new service?** → Create interface in `Application/Interfaces/`, register in Program.cs as scoped
2. **Modifying entities?** → Create migration via `dotnet ef migrations add`, run `dotnet ef database update`
3. **Adding validation?** → Add FluentValidator in `Application/Validators/`, auto-discovered in Program.cs
4. **Changing DB?** → Only modify Program.cs connection line and CustomWebApplicationFactory (tests)
5. **Adding API endpoint?** → Extract userId from claim, call service with userId, service handles authorization

## Environment Variables (Production Only)

```bash
JWT_KEY=your-256-bit-secret-key-here
ConnectionString=Server=...;Database=TaskManagerPro;...
ASPNETCORE_ENVIRONMENT=Production
```

Development uses hardcoded defaults in appsettings.Development.json.

## Documentation

- Full project overview: `README.md` (brief) → links to specific docs
- Backend setup: `src/MyApp/README.md`
- Frontend setup: `frontend/README.md`
- Database management: `DATABASE_SETUP.md`
- Testing guide: `tests/README.md`
