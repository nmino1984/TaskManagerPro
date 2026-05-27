namespace MyApp.Application.DTOs.TaskComment;

public class TaskCommentResponseDto
{
    public int TaskCommentId { get; set; }
    public int TaskId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? CreatedByUsername { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsEdited { get; set; }
    public int? ParentCommentId { get; set; }
    public List<TaskCommentResponseDto> Replies { get; set; } = new();
}
