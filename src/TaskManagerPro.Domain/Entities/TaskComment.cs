using System;

using System.Collections.Generic;
namespace TaskManagerPro.Domain.Entities;

public class TaskComment
{
    public int TaskCommentId { get; set; }
    public int TaskId { get; set; }
    public MyTask Task { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
    public User CreatedByUser { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsEdited { get; set; }
    public int? ParentCommentId { get; set; }
    public TaskComment? ParentComment { get; set; }
    public ICollection<TaskComment> Replies { get; set; } = new List<TaskComment>();
}

