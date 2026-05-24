# Frontend Issues Resolved - SubTask Creation

**Date:** 2026-05-24  
**Status:** ✅ RESOLVED

## Problem Statement
Users couldn't create subtasks from the Edit Task dialog. They received a **400 Bad Request** error.

## Root Causes Identified & Fixed

### 1. **Missing TaskId Validation** ❌ → ✅
**File:** `frontend/src/app/features/tasks/subtask-form/subtask-form.component.ts`

**Problem:**
```typescript
// BEFORE (unsafe)
if (!this.data) {  // Only checks if entire object is falsy
  this.data = { mode: 'create', taskId: 0 };
}
```
When `taskId` was undefined, it passed through as `{ mode: 'create', taskId: undefined }`

**Solution:**
```typescript
// AFTER (safe)
if (!this.data) {
  this.data = { mode: 'create', taskId: 0 };
}

if (!this.data.taskId) {
  this.error.set('Error: Task ID is missing. Cannot create subtask.');
}
```

### 2. **Enum Serialization Mismatch** ❌ → ✅
**File:** `frontend/src/app/features/tasks/subtask-form/subtask-form.component.ts`

**Problem:**
- Frontend was sending: `"status": "Pending"` (PascalCase)
- Backend expected: `"status": "pending"` (camelCase) due to `JsonNamingPolicy.CamelCase`
- Result: Deserialization error → 400 Bad Request

**Solution:**
```typescript
// BEFORE
status: formValue.status  // "Pending"

// AFTER
status: (formValue.status?.toLowerCase()) as any || 'pending'  // "pending"
```

### 3. **Invalid Date Validation** ❌ → ✅
**File:** `src/MyApp/Application/Validators/SubTaskValidator.cs`

**Problem:**
```csharp
// OLD (broken logic)
.GreaterThanOrEqualTo(x => DateTime.UtcNow)  // Syntax error in FluentValidation
```

Later changed to:
```csharp
// ATTEMPT (still problematic)
.Must(x => !x.HasValue || x.Value >= DateTime.UtcNow)
```

This rejected dates that were "in the past" - meaning users couldn't set dates for today or earlier.

**Solution:**
```csharp
// FIXED (removed validation entirely)
// DueDate can now be any valid date or null
```

**Rationale:** For an educational task management app, there's no business reason to reject past dates. Users might want to track historical subtasks or use dates as references.

---

## Testing the Fix

### Flow to Verify:
1. Start backend: `cd src/MyApp && dotnet run`
2. Start frontend: `cd frontend && npm start`
3. Create a task
4. Edit the task → Click **Subtasks** tab
5. Click **Create New Subtask**
6. Fill form:
   - Description: "My first subtask" (min 5 chars)
   - Status: "Pending"
   - Due Date: Any date (today, past, future all work)
   - Click **Create**

**Expected:** ✅ Subtask created successfully (snackbar appears)

---

## Files Modified

| File | Change | Reason |
|------|--------|--------|
| `subtask-form.component.ts` | Added taskId validation | Prevent undefined taskId from reaching backend |
| `subtask-form.component.ts` | Convert status to lowercase | Match backend's camelCase enum expectation |
| `SubTaskValidator.cs` | Removed date validation | Date constraints don't make sense for educational app |

---

## Architecture Notes for Next Claude

- **Validation Pattern:** Both frontend (UX) and backend (security) validate inputs
- **Enum Serialization:** Backend uses `JsonNamingPolicy.CamelCase`, so frontend must match
- **Error Handling:** ValidationFilter automatically converts backend validation errors to 400 responses
- **Multi-Tenancy:** Every service method receives userId; SubTaskService validates task ownership before allowing operations

---

## Related Components

- `SubTaskListComponent` → Manages list + opens create dialog
- `SubTaskFormComponent` → Form logic (this is what was fixed)
- `TaskFormComponent` → Parent that renders SubTaskListComponent
- `SubTaskService` → API calls to backend

Visual flow:
```
TaskListComponent
  ↓ click edit
TaskFormComponent (opens in dialog)
  ↓ click Subtasks tab
SubTaskListComponent
  ↓ click "Create New Subtask"
SubTaskFormComponent (opens in dialog)
  ↓ form submit
SubTaskService.create() → POST /api/v1/subtasks
```
