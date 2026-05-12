using MyApp.Domain.Enums;

namespace MyApp.Application.DTOs.MyTask;

public class TaskQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public MyTaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public string? Search { get; set; }
}
