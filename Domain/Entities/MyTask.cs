using MyApp.Domain.Enums;

namespace MyApp.Domain.Entities;

public class MyTask
{
    public int MyTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public MyTaskStatus Status { get; set; } = MyTaskStatus.NotStarted;

    public int Progress { get; set; } = 0;

    public List<SubTask> SubTasks { get; set; } = new();
    public List<CalendarEvent> CalendarEvents { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void UpdateProgress()
    {
        if (SubTasks.Count == 0)
        {
            Progress = 0;
            return;
        }

        int completed = SubTasks.Count(s => s.Status == SubTaskStatus.Completed);
        Progress = (int)((completed / (double)SubTasks.Count) * 100);

        UpdateStatus();
    }

    public void UpdateStatus()
    {
        if (Progress == 100)
        {
            Status = MyTaskStatus.Completed;
            return;
        }

        if (DateTime.UtcNow.Date > EndDate.Date)
        {
            Status = MyTaskStatus.Overdue;
            return;
        }

        Status = Progress > 0 ? MyTaskStatus.InProgress : MyTaskStatus.NotStarted;
    }
}
