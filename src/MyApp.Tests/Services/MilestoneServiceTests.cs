using Xunit;
using Moq;
using AutoMapper;
using MyApp.Application.Services;
using MyApp.Application.DTOs.Milestone;
using MyApp.Application.Exceptions;
using MyApp.Infrastructure.Data;
using MyApp.Domain.Entities;
using MyApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Tests.Services;

public class MilestoneServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IMapper> _mapperMock;
    private readonly MilestoneService _milestoneService;

    public MilestoneServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mapperMock = new Mock<IMapper>();
        _milestoneService = new MilestoneService(_context, _mapperMock.Object);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    [Fact]
    public async Task GetByTaskAsync_WithValidTaskId_ReturnsMilestones()
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

        var milestone = new Milestone
        {
            Title = "Milestone 1",
            Description = "Description",
            TaskId = task.MyTaskId,
            TargetDate = DateTime.UtcNow.AddDays(7),
            Status = MilestoneStatus.Pending
        };

        _context.Set<Milestone>().Add(milestone);
        await _context.SaveChangesAsync();

        var responseDto = new MilestoneResponseDto
        {
            MilestoneId = milestone.MilestoneId,
            TaskId = task.MyTaskId,
            Title = "Milestone 1",
            Status = MilestoneStatus.Pending
        };

        _mapperMock
            .Setup(m => m.Map<List<MilestoneResponseDto>>(It.IsAny<List<Milestone>>()))
            .Returns(new List<MilestoneResponseDto> { responseDto });

        // Act
        var result = await _milestoneService.GetByTaskAsync(task.MyTaskId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Milestone 1", result[0].Title);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_CreatesMilestone()
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

        var createDto = new MilestoneCreateDto
        {
            TaskId = task.MyTaskId,
            Title = "New Milestone",
            Description = "Description",
            TargetDate = DateTime.UtcNow.AddDays(14),
            Status = MilestoneStatus.Pending
        };

        var newMilestone = new Milestone
        {
            Title = "New Milestone",
            Description = "Description",
            TaskId = task.MyTaskId,
            TargetDate = DateTime.UtcNow.AddDays(14),
            Status = MilestoneStatus.Pending
        };

        var responseDto = new MilestoneResponseDto
        {
            MilestoneId = 1,
            TaskId = task.MyTaskId,
            Title = "New Milestone",
            Status = MilestoneStatus.Pending
        };

        _mapperMock
            .Setup(m => m.Map<Milestone>(It.IsAny<MilestoneCreateDto>()))
            .Returns(newMilestone);

        _mapperMock
            .Setup(m => m.Map<MilestoneResponseDto>(It.IsAny<Milestone>()))
            .Returns(responseDto);

        // Act
        var result = await _milestoneService.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Milestone", result.Title);
        Assert.Equal(MilestoneStatus.Pending, result.Status);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesMilestone()
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

        var milestone = new Milestone
        {
            Title = "Milestone to Delete",
            TaskId = task.MyTaskId,
            TargetDate = DateTime.UtcNow.AddDays(7),
            Status = MilestoneStatus.Pending
        };

        _context.Set<Milestone>().Add(milestone);
        await _context.SaveChangesAsync();

        var milestoneId = milestone.MilestoneId;

        // Act
        await _milestoneService.DeleteAsync(milestoneId);

        // Assert
        var deletedMilestone = await _context.Set<Milestone>().FirstOrDefaultAsync(m => m.MilestoneId == milestoneId);
        Assert.Null(deletedMilestone);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var milestoneId = 999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _milestoneService.GetByIdAsync(milestoneId)
        );
    }

    [Fact]
    public async Task UpdateAsync_WithValidDto_UpdatesMilestone()
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

        var milestone = new Milestone
        {
            Title = "Original Title",
            TaskId = task.MyTaskId,
            TargetDate = DateTime.UtcNow.AddDays(7),
            Status = MilestoneStatus.Pending
        };

        _context.Set<Milestone>().Add(milestone);
        await _context.SaveChangesAsync();

        var updateDto = new MilestoneUpdateDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            TargetDate = DateTime.UtcNow.AddDays(14),
            Status = MilestoneStatus.Completed
        };

        var responseDto = new MilestoneResponseDto
        {
            MilestoneId = milestone.MilestoneId,
            TaskId = task.MyTaskId,
            Title = "Updated Title",
            Status = MilestoneStatus.Completed
        };

        _mapperMock
            .Setup(m => m.Map<MilestoneResponseDto>(It.IsAny<Milestone>()))
            .Returns(responseDto);

        // Act
        var result = await _milestoneService.UpdateAsync(milestone.MilestoneId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal(MilestoneStatus.Completed, result.Status);
    }

}
