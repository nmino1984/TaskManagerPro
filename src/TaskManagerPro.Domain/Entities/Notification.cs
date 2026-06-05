using System;

namespace TaskManagerPro.Domain.Entities;

public class Notification
{
    public int NotificationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? RelatedTaskId { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public MyTask? RelatedTask { get; set; }
}
