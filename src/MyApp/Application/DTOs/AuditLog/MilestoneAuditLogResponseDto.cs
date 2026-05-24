namespace MyApp.Application.DTOs.AuditLog;

public class MilestoneAuditLogResponseDto
{
    public int MilestoneAuditLogId { get; set; }
    public int MilestoneId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangedByUserId { get; set; } = string.Empty;
    public string? ChangedByUsername { get; set; }
    public DateTime ChangedAt { get; set; }
}
