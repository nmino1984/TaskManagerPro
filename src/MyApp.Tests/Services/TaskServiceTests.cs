using Xunit;
using Moq;
using AutoMapper;
using MyApp.Application.Services;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.Exceptions;
using MyApp.Infrastructure.Data;
using MyApp.Domain.Entities;
using MyApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Tests.Services;

public class TaskServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mapperMock = new Mock<IMapper>();
        _taskService = new TaskService(_context, _mapperMock.Object);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidTaskId_ReturnsTask()
    {
        // Arrange
        var userId = "user-123";
        var task = new MyTask
        {
            Title = "Test Task",
            Description = "Test Description",
            UserId = userId,
            Status = MyTaskStatus.NotStarted,
            Priority = TaskPriority.Medium,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7)
        };

        _context.MyTasks.Add(task);
        await _context.SaveChangesAsync();

        var responseDto = new MyTaskResponseDto
        {
            MyTaskId = task.MyTaskId,
            Title = "Test Task",
            Status = MyTaskStatus.NotStarted
        };

        _mapperMock
            .Setup(m => m.Map<MyTaskResponseDto>(It.IsAny<MyTask>()))
            .Returns(responseDto);

        // Act
        var result = await _taskService.GetByIdAsync(task.MyTaskId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal(MyTaskStatus.NotStarted, result.Status);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_CreatesTask()
    {
        // Arrange
        var createDto = new MyTaskCreateDto
        {
            Title = "New Task",
            Description = "Description",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            Priority = TaskPriority.High,
            Status = MyTaskStatus.NotStarted
        };

        var userId = "user-123";
        var newTask = new MyTask
        {
            Title = "New Task",
            Description = "Description",
            UserId = userId,
            Status = MyTaskStatus.NotStarted,
            Priority = TaskPriority.High
        };

        var responseDto = new MyTaskResponseDto
        {
            MyTaskId = 1,
            Title = "New Task",
            Status = MyTaskStatus.NotStarted
        };

        _mapperMock
            .Setup(m => m.Map<MyTask>(It.IsAny<MyTaskCreateDto>()))
            .Returns(newTask);

        _mapperMock
            .Setup(m => m.Map<MyTaskResponseDto>(It.IsAny<MyTask>()))
            .Returns(responseDto);

        // Act
        var result = await _taskService.CreateAsync(createDto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Task", result.Title);
        Assert.Equal(MyTaskStatus.NotStarted, result.Status);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_SetsIsDeletedTrue()
    {
        // Arrange
        var userId = "user-123";
        var task = new MyTask
        {
            Title = "Task to Delete",
            Description = "Test",
            UserId = userId,
            Status = MyTaskStatus.NotStarted,
            Priority = TaskPriority.Low,
            IsDeleted = false
        };

        _context.MyTasks.Add(task);
        await _context.SaveChangesAsync();
        var taskId = task.MyTaskId;

        // Act
        await _taskService.DeleteAsync(taskId, userId);

        // Assert
        var deletedTask = await _context.MyTasks.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.MyTaskId == taskId);
        Assert.NotNull(deletedTask);
        Assert.True(deletedTask.IsDeleted);
        Assert.NotNull(deletedTask.DeletedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var taskId = 999;
        var userId = "user-123";

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.GetByIdAsync(taskId, userId)
        );
    }

    [Fact]
    public async Task UpdateAsync_WithValidDto_UpdatesTask()
    {
        // Arrange
        var userId = "user-123";
        var task = new MyTask
        {
            Title = "Original Title",
            Description = "Original Description",
            UserId = userId,
            Status = MyTaskStatus.NotStarted,
            Priority = TaskPriority.Low,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7)
        };

        _context.MyTasks.Add(task);
        await _context.SaveChangesAsync();
        var taskId = task.MyTaskId;

        var updateDto = new MyTaskUpdateDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Status = MyTaskStatus.InProgress,
            Priority = TaskPriority.High,
            EndDate = DateTime.UtcNow.AddDays(14)
        };

        var responseDto = new MyTaskResponseDto
        {
            MyTaskId = taskId,
            Title = "Updated Title",
            Status = MyTaskStatus.InProgress
        };

        _mapperMock
            .Setup(m => m.Map<MyTaskResponseDto>(It.IsAny<MyTask>()))
            .Returns(responseDto);

        // Act
        var result = await _taskService.UpdateAsync(taskId, updateDto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal(MyTaskStatus.InProgress, result.Status);
    }
}
