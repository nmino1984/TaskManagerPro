using System;
using TaskManagerPro.Application.DTOs.TaskComment;

using System.Collections.Generic;
using System.Threading.Tasks;
namespace TaskManagerPro.Application.Interfaces;

public interface ITaskCommentService
{
    Task<List<TaskCommentResponseDto>> GetByTaskAsync(int taskId, string userId);
    Task<TaskCommentResponseDto> CreateAsync(TaskCommentCreateDto dto, string userId);
    Task<TaskCommentResponseDto> UpdateAsync(int id, TaskCommentUpdateDto dto, string userId);
    Task DeleteAsync(int id, string userId);
}


