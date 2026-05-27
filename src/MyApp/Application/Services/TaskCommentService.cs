using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.TaskComment;
using MyApp.Application.Exceptions;
using MyApp.Application.Interfaces;
using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;
using System.Text.RegularExpressions;

namespace MyApp.Application.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public TaskCommentService(IUnitOfWork uow, IMapper mapper, INotificationService notificationService)
    {
        _uow = uow;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<List<TaskCommentResponseDto>> GetByTaskAsync(int taskId, string userId)
    {
        var task = await _uow.Tasks.GetByIdAsync(taskId)
            ?? throw new NotFoundException("MyTask", taskId);

        if (task.UserId != userId)
            throw new NotFoundException("MyTask", taskId);

        var comments = await _uow.TaskComments.GetByTaskIdAsync(taskId);
        return _mapper.Map<List<TaskCommentResponseDto>>(comments);
    }

    public async Task<TaskCommentResponseDto> CreateAsync(TaskCommentCreateDto dto, string userId)
    {
        var task = await _uow.Tasks.GetByIdAsync(dto.TaskId)
            ?? throw new NotFoundException("MyTask", dto.TaskId);

        if (task.UserId != userId)
            throw new UnauthorizedAccessException("You do not have access to this task.");

        var comment = new TaskComment
        {
            TaskId = dto.TaskId,
            Text = dto.Text,
            CreatedByUserId = userId,
            ParentCommentId = dto.ParentCommentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _uow.TaskComments.AddAsync(comment);
        await _uow.SaveChangesAsync();

        await HandleMentionsAsync(comment, userId);

        var result = await _uow.TaskComments.GetByIdAsync(comment.TaskCommentId);
        if (result == null)
            throw new InvalidOperationException("Failed to retrieve created comment");

        return _mapper.Map<TaskCommentResponseDto>(result);
    }

    public async Task<TaskCommentResponseDto> UpdateAsync(int id, TaskCommentUpdateDto dto, string userId)
    {
        var comment = await _uow.TaskComments.GetByIdAsync(id)
            ?? throw new NotFoundException("TaskComment", id);

        if (comment.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own comments.");

        comment.Text = dto.Text;
        comment.UpdatedAt = DateTime.UtcNow;
        comment.IsEdited = true;

        _uow.TaskComments.Update(comment);
        await _uow.SaveChangesAsync();

        await HandleMentionsAsync(comment, userId);

        var result = await _uow.TaskComments.GetByIdAsync(id);
        return _mapper.Map<TaskCommentResponseDto>(result!);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var comment = await _uow.TaskComments.GetByIdAsync(id)
            ?? throw new NotFoundException("TaskComment", id);

        var task = await _uow.Tasks.GetByIdAsync(comment.TaskId)
            ?? throw new NotFoundException("MyTask", comment.TaskId);

        if (comment.CreatedByUserId != userId && task.UserId != userId)
            throw new UnauthorizedAccessException("You cannot delete this comment.");

        _uow.TaskComments.Remove(comment);
        await _uow.SaveChangesAsync();
    }

    private async Task HandleMentionsAsync(TaskComment comment, string createdByUserId)
    {
        var mentionPattern = @"@(\w+)";
        var matches = Regex.Matches(comment.Text, mentionPattern);

        foreach (Match match in matches)
        {
            var mentionedUsername = match.Groups[1].Value;
            var mentionedUser = await _uow.Users.Query()
                .FirstOrDefaultAsync(u => u.Username == mentionedUsername);

            if (mentionedUser != null && mentionedUser.UserId != createdByUserId)
            {
                var task = await _uow.Tasks.GetByIdAsync(comment.TaskId);
                if (task != null)
                {
                    await _notificationService.CreateAsync(
                        mentionedUser.UserId,
                        "Comment Mention",
                        $"You were mentioned in a comment on task '{task.Title}'",
                        "Comment",
                        task.MyTaskId
                    );
                }
            }
        }
    }
}
