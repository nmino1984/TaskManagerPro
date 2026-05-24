namespace MyApp.Domain.Entities;

public class SubTaskAuditLog
{
    public int SubTaskAuditLogId { get; set; }

    public int SubTaskId { get; set; }
    public SubTask SubTask { get; set; } = null!;

    public string Action { get; set; } = string.Empty;

    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public string ChangedByUserId { get; set; } = string.Empty;
    public User ChangedByUser { get; set; } = null!;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
