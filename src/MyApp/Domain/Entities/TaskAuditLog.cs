namespace MyApp.Domain.Entities;

public class TaskAuditLog
{
    public int TaskAuditLogId { get; set; }

    public int TaskId { get; set; }
    public MyTask Task { get; set; } = null!;

    public string Action { get; set; } = string.Empty; // "Created" | "Updated" | "Deleted"

    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public string ChangedByUserId { get; set; } = string.Empty;
    public User ChangedByUser { get; set; } = null!;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
