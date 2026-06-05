using FluentAssertions;
using TaskManagerPro.Domain.Entities;
using TaskManagerPro.Domain.Enums;
using Xunit;

namespace TaskManagerPro.Unit.Tests.Domain;

public class MyTaskProgressTests
{
    private static MyTask CreateTask(DateTime? endDate = null) => new()
    {
        Title = "Test Task",
        StartDate = DateTime.UtcNow.AddDays(-5),
        EndDate = endDate ?? DateTime.UtcNow.AddDays(10)
    };

    private static SubTask Subtask(SubTaskStatus status) => new()
    {
        Description = "Test subtask",
        Status = status
    };

    // TODO: add edge case for task with 0 subtasks that was manually set to InProgress

    // --- Progress ---

    [Fact]
    public void UpdateProgress_NoSubtasks_SetsProgressToZero()
    {
        var task = CreateTask();
        task.UpdateProgress();
        task.Progress.Should().Be(0);
    }

    [Fact]
    public void all_completed_means_100_percent()
    {
        var task = CreateTask();
        task.SubTasks.Add(Subtask(SubTaskStatus.Completed));
        task.SubTasks.Add(Subtask(SubTaskStatus.Completed));
        task.UpdateProgress();
        task.Progress.Should().Be(100);
    }

    [Fact]
    public void UpdateProgress_OneOfThreeCompleted_SetsProgress33()
    {
        var task = CreateTask();
        task.SubTasks.Add(Subtask(SubTaskStatus.Completed));
        task.SubTasks.Add(Subtask(SubTaskStatus.Pending));
        task.SubTasks.Add(Subtask(SubTaskStatus.Pending));
        task.UpdateProgress();
        task.Progress.Should().Be(33);
    }

    [Fact]
    public void UpdateProgress_TwoOfFourCompleted_SetsProgress50()
    {
        var task = CreateTask();
        task.SubTasks.Add(Subtask(SubTaskStatus.Completed));
        task.SubTasks.Add(Subtask(SubTaskStatus.Completed));
        task.SubTasks.Add(Subtask(SubTaskStatus.Pending));
        task.SubTasks.Add(Subtask(SubTaskStatus.Pending));
        task.UpdateProgress();
        task.Progress.Should().Be(50);
    }

    [Fact]
    public void UpdateProgress_TwoOfThreeCompleted_SetsProgress66()
    {
        var task = CreateTask();
        task.SubTasks.Add(Subtask(SubTaskStatus.Completed));
        task.SubTasks.Add(Subtask(SubTaskStatus.Completed));
        task.SubTasks.Add(Subtask(SubTaskStatus.Pending));
        task.UpdateProgress();
        task.Progress.Should().Be(66);
    }

    // --- Status ---

    [Fact]
    public void no_subtasks_future_deadline_stays_not_started()
    {
        var task = CreateTask(endDate: DateTime.UtcNow.AddDays(10));
        task.UpdateProgress();
        task.Status.Should().Be(MyTaskStatus.NotStarted);
    }

    [Fact]
    public void UpdateStatus_NoSubtasks_PastEndDate_SetsOverdue()
    {
        var task = CreateTask(endDate: DateTime.UtcNow.AddDays(-1));
        task.UpdateProgress();
        task.Status.Should().Be(MyTaskStatus.Overdue);
    }

    [Fact]
    public void completed_even_if_past_due_date()
    {
        var task = CreateTask(endDate: DateTime.UtcNow.AddDays(-5));
        task.SubTasks.Add(Subtask(SubTaskStatus.Completed));
        task.UpdateProgress();
        task.Status.Should().Be(MyTaskStatus.Completed);
    }

    [Fact]
    public void UpdateStatus_PartialCompletion_FutureEndDate_SetsInProgress()
    {
        var task = CreateTask(endDate: DateTime.UtcNow.AddDays(10));
        task.SubTasks.Add(Subtask(SubTaskStatus.Completed));
        task.SubTasks.Add(Subtask(SubTaskStatus.Pending));
        task.UpdateProgress();
        task.Status.Should().Be(MyTaskStatus.InProgress);
    }

    [Fact]
    public void overdue_if_partial_and_past_deadline()
    {
        var task = CreateTask(endDate: DateTime.UtcNow.AddDays(-1));
        task.SubTasks.Add(Subtask(SubTaskStatus.Completed));
        task.SubTasks.Add(Subtask(SubTaskStatus.Pending));
        task.UpdateProgress();
        task.Status.Should().Be(MyTaskStatus.Overdue);
    }

    [Fact]
    public void UpdateStatus_AllPending_PastEndDate_SetsOverdue()
    {
        var task = CreateTask(endDate: DateTime.UtcNow.AddDays(-1));
        task.SubTasks.Add(Subtask(SubTaskStatus.Pending));
        task.SubTasks.Add(Subtask(SubTaskStatus.Pending));
        task.UpdateProgress();
        task.Status.Should().Be(MyTaskStatus.Overdue);
    }

    // not used yet but might be handy for future overdue-notification tests
    private static MyTask OverdueTask() => CreateTask(endDate: DateTime.UtcNow.AddDays(-30));
}
