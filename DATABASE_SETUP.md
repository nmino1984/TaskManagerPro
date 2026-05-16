# TaskManagerPro - Database Setup Guide

## Overview

TaskManagerPro uses SQLite for development and local testing. The database file (`TaskManagerPro.db`) is **not committed to Git** to avoid conflicts and data pollution.

## Initial Setup

### Option A: Use the Example Database (Recommended for Testing)

A pre-configured database is included in the repository: `TaskManagerPro.db.example`

**To use it:**

#### Linux/macOS:
```bash
cd src/MyApp
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
dotnet run
```

#### Windows (PowerShell):
```powershell
cd src/MyApp
Copy-Item ../../TaskManagerPro.db.example -Destination ./TaskManagerPro.db
dotnet run
```

**What's in the example database:**
- All tables created and ready to use
- Full schema with indices for performance
- Optional: Seed data for immediate testing

This is the **fastest way to get started** - the database is ready immediately.

### Option B: Auto-Create Fresh Database

If you want a completely fresh database, the application can create one automatically:

```bash
cd src/MyApp
dotnet run
```

The application will:
- Create `TaskManagerPro.db` automatically via Entity Framework migrations
- Initialize all tables (Users, MyTasks, SubTasks, Milestones, etc.)
- Create indices for performance optimization
- Load seed data (development mode only)

**This takes 2-3 seconds longer than Option A.**

### 2. Seed Data (Optional)

If you want to start with test data, the application includes a `DataSeeder` that runs in **Development mode only**:

```csharp
// Program.cs - automatically runs on first launch in Development
if (app.Environment.IsDevelopment())
{
    await DataSeeder.SeedAsync(db);
}
```

**Seed data includes:**
- Demo user (username: `demo`, password in seed)
- Sample tasks with various states
- Sample subtasks and milestones

### 3. Creating a Test User

You can create users through the API:

```bash
curl -X POST http://localhost:5141/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "TestPassword123!"
  }'
```

## Adding Data to the Database

### Method 1: Through the Application UI

1. Start the backend: `dotnet run` (in `src/MyApp`)
2. Start the frontend: `npm start` (in `frontend`)
3. Open `http://localhost:4200`
4. Register a new user
5. Click "Add Task" and create tasks manually
6. Add subtasks and milestones through the UI

**Advantages:**
- Visual interface
- See changes in real-time
- Understand the application workflow

### Method 2: Through the API

Use curl or Postman to directly call the API:

#### Register a user:
```bash
curl -X POST http://localhost:5141/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "TestPassword123!"
  }'
```

**Save the returned `token` for the next requests.**

#### Create a task:
```bash
curl -X POST http://localhost:5141/api/v1/tasks \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "title": "My First Task",
    "description": "Task description",
    "startDate": "2026-05-16T00:00:00Z",
    "endDate": "2026-06-16T00:00:00Z",
    "priority": "High"
  }'
```

**Save the returned `myTaskId`.**

#### Create a subtask:
```bash
curl -X POST http://localhost:5141/api/v1/subtasks \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "taskId": TASK_ID_HERE,
    "description": "Subtask description"
  }'
```

#### Create a milestone:
```bash
curl -X POST http://localhost:5141/api/v1/milestones \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "taskId": TASK_ID_HERE,
    "title": "Milestone Title",
    "description": "Important checkpoint",
    "targetDate": "2026-06-01T00:00:00Z",
    "status": "Pending"
  }'
```

### Method 3: Use the Seed Data Script

The application automatically loads seed data in **Development mode**:

```bash
# Ensure you're in Development environment
set ASPNETCORE_ENVIRONMENT=Development  # Windows
export ASPNETCORE_ENVIRONMENT=Development  # Linux/macOS

cd src/MyApp
dotnet run
```

The `DataSeeder` class will automatically populate the database with:
- Sample tasks with various states
- Sample subtasks
- Sample milestones

**See**: `src/MyApp/Infrastructure/Data/DataSeeder.cs`

---

## Clearing Data

### Method 1: Delete All User Data (Keep Schema)

**WARNING: This deletes all your data. Backup first if needed.**

There's no built-in API endpoint to delete all data, so use the database directly:

#### Via SQLite CLI:
```bash
# Connect to database
sqlite3 src/MyApp/TaskManagerPro.db

# Delete all data (keeps tables/schema)
DELETE FROM Milestones;
DELETE FROM SubTasks;
DELETE FROM MyTasks;
DELETE FROM Users;

# Verify (should show empty)
SELECT COUNT(*) FROM Users;

# Exit
.exit
```

#### Via PowerShell (Windows):
```powershell
# Requires sqlite3.exe installed
sqlite3 "src/MyApp/TaskManagerPro.db" "DELETE FROM Milestones; DELETE FROM SubTasks; DELETE FROM MyTasks; DELETE FROM Users;"
```

### Method 2: Delete Specific User's Data

Delete only one user's tasks (more targeted):

```bash
sqlite3 src/MyApp/TaskManagerPro.db
DELETE FROM MyTasks WHERE UserId = 'user-id-here';
.exit
```

### Method 3: Soft Delete (Soft Delete Flag)

Tasks use **soft delete** (not physically removed). To view deleted tasks:

```bash
sqlite3 src/MyApp/TaskManagerPro.db
SELECT * FROM MyTasks WHERE IsDeleted = 1;
.exit
```

To permanently delete soft-deleted records:

```bash
sqlite3 src/MyApp/TaskManagerPro.db
DELETE FROM MyTasks WHERE IsDeleted = 1;
.exit
```

### Method 4: Complete Database Reset

To start completely fresh:

#### Linux/macOS:
```bash
cd src/MyApp
rm TaskManagerPro.db TaskManagerPro.db-shm TaskManagerPro.db-wal 2>/dev/null
dotnet run
```

**This will:**
1. Delete the current database
2. Create a fresh one via migrations
3. Load seed data automatically (Development mode)

#### Windows (PowerShell):
```powershell
cd src/MyApp
Remove-Item TaskManagerPro.db, TaskManagerPro.db-shm, TaskManagerPro.db-wal -ErrorAction SilentlyContinue
dotnet run
```

#### Or restore from example:
```bash
cd src/MyApp
cp ../../TaskManagerPro.db.example ./TaskManagerPro.db
rm TaskManagerPro.db-shm TaskManagerPro.db-wal 2>/dev/null  # Remove WAL files
dotnet run
```

---

## Database Reset

To start with a fresh database:

### Option A: Delete and Regenerate (Linux/macOS)
```bash
cd src/MyApp
rm TaskManagerPro.db TaskManagerPro.db-shm TaskManagerPro.db-wal 2>/dev/null
dotnet run
```

### Option B: Delete and Regenerate (Windows PowerShell)
```powershell
cd src/MyApp
Remove-Item TaskManagerPro.db, TaskManagerPro.db-shm, TaskManagerPro.db-wal -ErrorAction SilentlyContinue
dotnet run
```

The database will be recreated automatically with migrations and seed data.

## Database Location

The SQLite database is located at:
```
src/MyApp/TaskManagerPro.db
```

Associated WAL (Write-Ahead Logging) files:
- `src/MyApp/TaskManagerPro.db-shm` - Shared memory file
- `src/MyApp/TaskManagerPro.db-wal` - Write-ahead log

**All these files are ignored by Git** (.gitignore) and should not be committed.

## Data Seeding

The `DataSeeder` class (in `Infrastructure/Data/DataSeeder.cs`) provides initial test data. It runs automatically in Development:

### Seed Data Structure:
- **Users**: Demo user for testing
- **Tasks**: Multiple tasks with different priorities and states
- **SubTasks**: Tasks broken down into subtasks
- **Milestones**: Important checkpoints for each task

### To modify seed data:
Edit `src/MyApp/Infrastructure/Data/DataSeeder.cs` and restart the application.

## Database Schema

Entity Framework Core handles schema management through **migrations**:

```bash
# View migrations
ls src/MyApp/Infrastructure/Migrations/

# Create new migration (if you modify models)
cd src/MyApp
dotnet ef migrations add YourMigrationName

# Apply migrations
dotnet ef database update
```

**Never manually edit the database schema** - use EF Core migrations instead.

## Development vs Production

### Development (SQLite)
- **Location**: `src/MyApp/TaskManagerPro.db`
- **Auto-migration**: Yes (automatic on startup)
- **Auto-seed**: Yes (in Development environment only)
- **Recreate**: Delete .db file and restart

### Production (SQL Server/Other)
- Configure connection string in environment variables
- Run migrations via: `dotnet ef database update`
- Use production database backup/restore procedures

## Troubleshooting

### Database is locked
**Problem**: "database is locked" error
**Solution**: 
- Close all connections to the database
- Delete `.db-shm` and `.db-wal` files
- Restart the application

### Migrations out of sync
**Problem**: Migration errors on startup
**Solution**:
```bash
cd src/MyApp
dotnet ef database drop -f
dotnet ef database update
dotnet run
```

### Can't connect to database
**Problem**: Connection refused
**Solution**:
- Verify `appsettings.json` has correct connection string
- Ensure write permissions to `src/MyApp/` directory
- Check that no other process is locking the database file

## Environment Variables

No database environment variables needed for development - SQLite uses local file path configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=TaskManagerPro.db"
  }
}
```

For production, set:
```bash
export ConnectionString="your-production-connection-string"
```

## Integration Tests

Integration tests use **in-memory SQLite** (`:memory:`) and do not use the file-based database:

```csharp
// CustomWebApplicationFactory.cs
var connection = new SqliteConnection("Data Source=:memory:");
```

Each test run gets a fresh, isolated database. No cleanup needed.

## Next Steps

1. ✅ Run backend: `dotnet run` (database created automatically)
2. ✅ Run frontend: `npm start` (in `frontend/` directory)
3. ✅ Access at: `http://localhost:4200`
4. ✅ Register a test user or use seed data
5. ✅ Create tasks, subtasks, and milestones

For questions about database setup, see the main [README.md](README.md).
