namespace MyApp.Application.DTOs.SubTask;

public class SubTaskResponseDto
{
    public int SubTaskId { get; set; }
    public int TaskId { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pendiente";

    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
