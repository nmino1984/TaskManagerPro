namespace MyApp.Domain.Entities;

public class Task
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Allowed values: "High", "Medium", "Low"
    public string Priority { get; set; } = "Medium";

    // Allowed values: "NotStarted", "InProgress", "Completed", "Overdue"
    public string Status { get; set; } = "NotStarted";

    public int Progress { get; set; } = 0;

    public List<SubTask> SubTasks { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void UpdateProgress()
    {
        if (SubTasks.Count == 0)
        {
            Progress = 0;
            return;
        }

        int completed = SubTasks.Count(s => s.Status == "Completed");
        Progress = (int)((completed / (double)SubTasks.Count) * 100);

        UpdateStatus();
    }

    public void UpdateStatus()
    {
        if (Progress == 100)
        {
            Status = "Completed";
            return;
        }

        if (DateTime.UtcNow.Date > EndDate.Date)
        {
            Status = "Overdue";
            return;
        }

        if (Progress > 0)
        {
            Status = "InProgress";
            return;
        }

        Status = "NotStarted";
    }
}
