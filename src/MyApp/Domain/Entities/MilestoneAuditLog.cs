namespace MyApp.Domain.Entities;

public class MilestoneAuditLog
{
    public int MilestoneAuditLogId { get; set; }

    public int MilestoneId { get; set; }
    public Milestone Milestone { get; set; } = null!;

    public string Action { get; set; } = string.Empty;

    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public string ChangedByUserId { get; set; } = string.Empty;
    public User ChangedByUser { get; set; } = null!;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
