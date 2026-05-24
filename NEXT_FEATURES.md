# Next Features to Implement

**Current Foundation:** ✅ Core CRUD + Notifications + Clean Architecture  
**Recommended Next:** Pick ONE and go deep

---

## Option 1: Task Assignment (⭐ Recommended First)

**What:** Users can assign tasks to other users  
**Why:** Most realistic, very useful, teaches multi-tenancy edge cases  
**Effort:** 6-8 hours  
**Skill Growth:** Permissions, relationships, notifications

### Implementation Outline:
```
Domain Layer:
- Add property `AssignedToUserId` (nullable) to MyTask entity

Migrations:
- Add column AssignedToUserId to MyTask table
- Add index on (UserId, AssignedToUserId)

API Layer:
- PATCH /api/v1/tasks/{id}/assign
  Body: { "assignToUserId": "user-id" }

Service Layer:
- TaskService.AssignAsync(taskId, assignToUserId, currentUserId)
  - Validate current user owns the task
  - Validate assignee exists
  - Update AssignedToUserId
  - Create notification "Task assigned to you"

Frontend:
- Add "Assign to" dropdown in task edit form
- Fetch list of other users from new endpoint
- Show "Assigned to: [user]" badge in task list

Notifications:
- Hangfire job triggers when task is assigned
- Send notification to assigned user
```

### Database Change:
```sql
ALTER TABLE "MyTasks" ADD COLUMN "AssignedToUserId" TEXT;
ALTER TABLE "MyTasks" ADD CONSTRAINT FK_MyTasks_AssignedToUserId 
  FOREIGN KEY ("AssignedToUserId") REFERENCES "Users"("Id");
```

---

## Option 2: Task History/Audit Trail

**What:** Track every change made to a task  
**Why:** Compliance, debugging, "who broke what"  
**Effort:** 4-5 hours  
**Skill Growth:** Event sourcing pattern, auditing

### Implementation Outline:
```
Domain Layer:
- Create TaskAudit entity:
  * AuditId (PK)
  * TaskId (FK)
  * UserId (who changed it)
  * ChangeType (Created, Updated, Deleted, StatusChanged, etc)
  * OldValue (JSON of old state)
  * NewValue (JSON of new state)
  * ChangedAt (timestamp)
  * ChangedProperty (which field changed)

Service Layer:
- Create AuditService.LogChangeAsync()
- Call it in TaskService after every operation
- Pattern: Before/After snapshot comparison

API Layer:
- GET /api/v1/tasks/{id}/audit
  Returns list of all changes with timestamps

Frontend:
- Show "History" tab next to Task Info/Subtasks
- Timeline view of changes
- "Edited by [user] at [time]: [field] changed from [old] to [new]"
```

### Example Audit Entry:
```json
{
  "auditId": 1,
  "taskId": 5,
  "userId": "user-123",
  "changeType": "Updated",
  "changedProperty": "Status",
  "oldValue": "NotStarted",
  "newValue": "InProgress",
  "changedAt": "2026-05-24T15:30:00Z"
}
```

---

## Option 3: Task Comments

**What:** Users can leave comments on tasks  
**Why:** Collaboration, discussion, very practical  
**Effort:** 8-10 hours  
**Skill Growth:** Nested comments, real-time features, mentions

### Implementation Outline:
```
Domain Layer:
- Create TaskComment entity:
  * CommentId
  * TaskId
  * UserId (who commented)
  * Content (text)
  * CreatedAt
  * UpdatedAt
  * ParentCommentId (for replies)

API Layer:
- POST /api/v1/tasks/{id}/comments (create)
- GET /api/v1/tasks/{id}/comments (list)
- PUT /api/v1/comments/{id} (edit own)
- DELETE /api/v1/comments/{id} (delete own)

Service:
- Validate task ownership (can comment on any shared task)
- Validate user can only edit/delete own comments
- Trigger notification when commented on

Frontend:
- Comments section below task form
- Thread view (replies nested under parent)
- Real-time updates via WebSocket or polling
- @mention autocomplete

Future:
- Markdown support
- Attachment uploads
- Reactions (👍 👎)
```

---

## Option 4: Task Dependencies

**What:** Task B cannot start until Task A is done  
**Why:** Complex project management, teaches graph algorithms  
**Effort:** 10-12 hours  
**Skill Growth:** Dependency graphs, cycle detection, critical path

### Implementation Outline:
```
Domain Layer:
- Create TaskDependency entity:
  * DependencyId
  * TaskId (dependent task)
  * DependsOnTaskId (prerequisite task)
  * DependencyType (BlockedBy, Triggers, etc)

Validation:
- Prevent cycles (A→B→C→A)
- Prevent self-dependency
- Validate both tasks belong to same user

Service:
- Cannot mark task as complete if dependencies not met
- Auto-cascade completions (if A complete, auto-complete B)

Frontend:
- Dependency graph visualization
- Show "blocked by" status
- Warn user if trying to complete with unmet dependencies
- Drag-and-drop to add dependencies
```

---

## Comparison Table

| Feature | Effort | Complexity | Real-World Use | Learning Value |
|---------|--------|-----------|-----------------|-----------------|
| **Task Assignment** | 6-8h | Medium | ⭐⭐⭐⭐⭐ | Permissions, relationships |
| **Audit Trail** | 4-5h | Easy | ⭐⭐⭐⭐ | Event logging, snapshots |
| **Comments** | 8-10h | Medium | ⭐⭐⭐⭐⭐ | Nested data, real-time |
| **Dependencies** | 10-12h | Hard | ⭐⭐⭐ | Graphs, algorithms |

---

## My Recommendation

**Start with Task Assignment (#1)** because:
1. Most immediately useful (team workflows)
2. Teaches edge cases of multi-tenancy (can only assign tasks you own)
3. Requires notifications integration (already working with Hangfire)
4. Foundation for future "shared task" features
5. Not too complex, not too simple - perfect learning difficulty

**Then do Audit Trail (#2)** because:
1. Quick to implement
2. Good foundation for compliance/debugging
3. Natural prerequisite for "undo" features later
4. Very common enterprise pattern

---

## Getting Started

When ready, say:
- **"Let's do Task Assignment"** → I'll create a detailed implementation plan
- **"Let's do Audit Trail"** → I'll create database migrations + service structure
- **"Let's do Comments"** → I'll set up the entity + API endpoints
- **"Let's do Dependencies"** → I'll design the graph validation logic

Which interests you most? 🚀
