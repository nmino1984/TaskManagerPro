namespace MyApp.Domain.Entities;

public class SubTask
{
    public int SubTaskId { get; set; }
    public int TaskId { get; set; }

    public string Description { get; set; } = string.Empty;

    // Allowed values: "Pending", "Completed"
    public string Status { get; set; } = "Pending";

    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
