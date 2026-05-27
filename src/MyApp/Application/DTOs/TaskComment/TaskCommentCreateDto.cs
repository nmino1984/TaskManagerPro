namespace MyApp.Application.DTOs.TaskComment;

public class TaskCommentCreateDto
{
    public int TaskId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int? ParentCommentId { get; set; }
}
