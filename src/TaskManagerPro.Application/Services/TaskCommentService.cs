using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.TaskComment;
using TaskManagerPro.Application.Exceptions;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;

namespace TaskManagerPro.Application.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    // TODO: paginate GetByTaskAsync — active tasks accumulate comments fast
    // FIXME: HandleMentionsAsync fires on edit but doesn't deduplicate — users get pinged twice if mentioned again
    // TODO: consider rate-limiting comment creation per user to prevent spam

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
        {
            throw new NotFoundException("MyTask", taskId);
        }

        var comments = await _uow.TaskComments.GetByTaskIdAsync(taskId);
        return _mapper.Map<List<TaskCommentResponseDto>>(comments);
    }

    public async Task<TaskCommentResponseDto> CreateAsync(TaskCommentCreateDto dto, string userId)
    {
        var task = await _uow.Tasks.GetByIdAsync(dto.TaskId)
            ?? throw new NotFoundException("MyTask", dto.TaskId);

        if (task.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have access to this task.");
        }

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
        {
            throw new InvalidOperationException("Failed to retrieve created comment");
        }

        return _mapper.Map<TaskCommentResponseDto>(result);
    }

    public async Task<TaskCommentResponseDto> UpdateAsync(int id, TaskCommentUpdateDto dto, string userId)
    {
        var comment = await _uow.TaskComments.GetByIdAsync(id)
            ?? throw new NotFoundException("TaskComment", id);

        if (comment.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("You can only edit your own comments.");
        }

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
        {
            throw new UnauthorizedAccessException("You cannot delete this comment.");
        }

        _uow.TaskComments.Remove(comment);
        await _uow.SaveChangesAsync();
    }

    private bool ContainsMentions(string text) => Regex.IsMatch(text, @"@\w+");

    private async Task HandleMentionsAsync(TaskComment comment, string createdByUserId)
    {
        var mentionPattern = @"@(\w+)";
        var matches = Regex.Matches(comment.Text, mentionPattern);

        foreach (Match match in matches)
        {
            var mentionedUsername = match.Groups[1].Value;
            var mentionedUser = await _uow.Users.GetByUsernameAsync(mentionedUsername);

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



