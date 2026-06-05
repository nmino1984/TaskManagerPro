namespace TaskManagerPro.Application.DTOs.MyTask;

public class AssignTaskDto
{
    public string? AssignToUserId { get; set; }
}

public class TaskAssignmentResponseDto
{
    public int MyTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? AssignedToUserId { get; set; }
    public string? AssignedToUsername { get; set; }
}
