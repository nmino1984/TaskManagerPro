using Xunit;
using Moq;
using AutoMapper;
using MyApp.Application.Services;
using MyApp.Application.DTOs.SubTask;
using MyApp.Application.Exceptions;
using MyApp.Infrastructure.Data;
using MyApp.Domain.Entities;
using MyApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Tests.Services;

public class SubTaskServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IMapper> _mapperMock;
    private readonly SubTaskService _subTaskService;

    public SubTaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mapperMock = new Mock<IMapper>();
        _subTaskService = new SubTaskService(_context, _mapperMock.Object);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    [Fact]
    public async Task GetByTaskAsync_WithValidTaskId_ReturnsSubTasks()
    {
        // Arrange
        var userId = "user-123";
        var task = new MyTask
        {
            Title = "Test Task",
            UserId = userId,
            Status = MyTaskStatus.NotStarted,
            Priority = TaskPriority.Medium
        };

        _context.MyTasks.Add(task);
        await _context.SaveChangesAsync();

        var subTask = new SubTask
        {
            Description = "Sub Task 1",
            TaskId = task.MyTaskId,
            Status = SubTaskStatus.Pending
        };

        _context.SubTasks.Add(subTask);
        await _context.SaveChangesAsync();

        var responseDto = new SubTaskResponseDto
        {
            SubTaskId = subTask.SubTaskId,
            TaskId = task.MyTaskId,
            Description = "Sub Task 1",
            Status = SubTaskStatus.Pending
        };

        _mapperMock
            .Setup(m => m.Map<List<SubTaskResponseDto>>(It.IsAny<List<SubTask>>()))
            .Returns(new List<SubTaskResponseDto> { responseDto });

        // Act
        var result = await _subTaskService.GetByTaskAsync(task.MyTaskId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Sub Task 1", result[0].Description);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_CreatesSubTask()
    {
        // Arrange
        var userId = "user-123";
        var task = new MyTask
        {
            Title = "Test Task",
            UserId = userId,
            Status = MyTaskStatus.NotStarted,
            Priority = TaskPriority.Medium
        };

        _context.MyTasks.Add(task);
        await _context.SaveChangesAsync();

        var createDto = new SubTaskCreateDto
        {
            TaskId = task.MyTaskId,
            Description = "New SubTask",
            Status = SubTaskStatus.Pending
        };

        var newSubTask = new SubTask
        {
            Description = "New SubTask",
            TaskId = task.MyTaskId,
            Status = SubTaskStatus.Pending
        };

        var responseDto = new SubTaskResponseDto
        {
            SubTaskId = 1,
            TaskId = task.MyTaskId,
            Description = "New SubTask",
            Status = SubTaskStatus.Pending
        };

        _mapperMock
            .Setup(m => m.Map<SubTask>(It.IsAny<SubTaskCreateDto>()))
            .Returns(newSubTask);

        _mapperMock
            .Setup(m => m.Map<SubTaskResponseDto>(It.IsAny<SubTask>()))
            .Returns(responseDto);

        // Act
        var result = await _subTaskService.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New SubTask", result.Description);
        Assert.Equal(SubTaskStatus.Pending, result.Status);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesSubTask()
    {
        // Arrange
        var userId = "user-123";
        var task = new MyTask
        {
            Title = "Test Task",
            UserId = userId,
            Status = MyTaskStatus.NotStarted,
            Priority = TaskPriority.Medium
        };

        _context.MyTasks.Add(task);
        await _context.SaveChangesAsync();

        var subTask = new SubTask
        {
            Description = "Sub Task to Delete",
            TaskId = task.MyTaskId,
            Status = SubTaskStatus.Pending
        };

        _context.SubTasks.Add(subTask);
        await _context.SaveChangesAsync();

        var subTaskId = subTask.SubTaskId;

        // Act
        await _subTaskService.DeleteAsync(subTaskId);

        // Assert
        var deletedSubTask = await _context.SubTasks.FirstOrDefaultAsync(s => s.SubTaskId == subTaskId);
        Assert.Null(deletedSubTask);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var subTaskId = 999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _subTaskService.GetByIdAsync(subTaskId)
        );
    }

    [Fact]
    public async Task UpdateAsync_WithValidDto_UpdatesSubTask()
    {
        // Arrange
        var userId = "user-123";
        var task = new MyTask
        {
            Title = "Test Task",
            UserId = userId,
            Status = MyTaskStatus.NotStarted,
            Priority = TaskPriority.Medium
        };

        _context.MyTasks.Add(task);
        await _context.SaveChangesAsync();

        var subTask = new SubTask
        {
            Description = "Original Description",
            TaskId = task.MyTaskId,
            Status = SubTaskStatus.Pending
        };

        _context.SubTasks.Add(subTask);
        await _context.SaveChangesAsync();

        var updateDto = new SubTaskUpdateDto
        {
            Description = "Updated Description",
            Status = SubTaskStatus.Completed
        };

        var responseDto = new SubTaskResponseDto
        {
            SubTaskId = subTask.SubTaskId,
            TaskId = task.MyTaskId,
            Description = "Updated Description",
            Status = SubTaskStatus.Completed
        };

        _mapperMock
            .Setup(m => m.Map<SubTaskResponseDto>(It.IsAny<SubTask>()))
            .Returns(responseDto);

        // Act
        var result = await _subTaskService.UpdateAsync(subTask.SubTaskId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal(SubTaskStatus.Completed, result.Status);
    }
}
