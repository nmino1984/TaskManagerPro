using System;

namespace TaskManagerPro.Application.DTOs.AuditLog;

public class SubTaskAuditLogResponseDto
{
    public int SubTaskAuditLogId { get; set; }
    public int SubTaskId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangedByUserId { get; set; } = string.Empty;
    public string? ChangedByUsername { get; set; }
    public DateTime ChangedAt { get; set; }
}
