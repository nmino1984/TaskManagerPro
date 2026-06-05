using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaskManagerPro.Application.DTOs.Milestone;
using TaskManagerPro.Application.Exceptions;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;
using TaskManagerPro.Domain.Enums;
namespace TaskManagerPro.Application.Services;

public class MilestoneService : IMilestoneService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    // TODO: add pagination support to GetByTaskAsync — tasks with many milestones hit this hard
    // TODO: cache completion stats per task, dashboard calls this on every load
    // FIXME: EscapeICalValue doesn't fold long lines per RFC 5545 — will break some calendar clients

    public MilestoneService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<MilestoneResponseDto>> GetByTaskAsync(int taskId, string userId)
    {
        var taskExists = await _uow.Tasks.ExistsForUserAsync(taskId, userId);

        if (!taskExists)
        {
            throw new NotFoundException("MyTask", taskId);
        }

        var milestones = await _uow.Milestones.GetByTaskIdAsync(taskId);

        return _mapper.Map<List<MilestoneResponseDto>>(milestones);
    }

    public async Task<MilestoneResponseDto> GetByIdAsync(int id, string userId)
    {
        var milestone = await _uow.Milestones.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException("Milestone", id);

        return _mapper.Map<MilestoneResponseDto>(milestone);
    }

    public async Task<MilestoneResponseDto> CreateAsync(MilestoneCreateDto dto, string userId)
    {
        var task = await _uow.Tasks.GetByIdAsync(dto.TaskId)
            ?? throw new NotFoundException("MyTask", dto.TaskId);

        if (task.UserId != userId)
        {
            throw new NotFoundException("MyTask", dto.TaskId);
        }

        var milestone = _mapper.Map<Milestone>(dto);
        milestone.CreatedAt = DateTime.UtcNow;
        milestone.UpdatedAt = DateTime.UtcNow;

        await _uow.Milestones.AddAsync(milestone);
        await _uow.SaveChangesAsync();

        await WriteAuditLogsAsync(milestone.MilestoneId, userId, "Created",
            new List<(string, string?, string?)> { ("Title", null, milestone.Title) });
        await _uow.SaveChangesAsync();

        return _mapper.Map<MilestoneResponseDto>(milestone);
    }

    public async Task<MilestoneResponseDto> UpdateAsync(int id, MilestoneUpdateDto dto, string userId)
    {
        var milestone = await _uow.Milestones.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException("Milestone", id);

        var oldTitle = milestone.Title;
        var oldDescription = milestone.Description;
        var oldTargetDate = milestone.TargetDate;
        var oldStatus = milestone.Status;

        _mapper.Map(dto, milestone);
        milestone.UpdatedAt = DateTime.UtcNow;

        _uow.Milestones.Update(milestone);

        var changes = new List<(string, string?, string?)>();
        if (oldTitle != milestone.Title)
        {
            changes.Add(("Title", oldTitle, milestone.Title));
        }

        if (oldDescription != milestone.Description)
        {
            changes.Add(("Description", oldDescription, milestone.Description));
        }

        if (oldTargetDate != milestone.TargetDate)
        {
            changes.Add(("TargetDate", oldTargetDate.ToString("O"), milestone.TargetDate.ToString("O")));
        }

        if (oldStatus != milestone.Status)
        {
            changes.Add(("Status", oldStatus.ToString(), milestone.Status.ToString()));
        }

        await WriteAuditLogsAsync(milestone.MilestoneId, userId, "Updated", changes);
        await _uow.SaveChangesAsync();

        return _mapper.Map<MilestoneResponseDto>(milestone);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var milestone = await _uow.Milestones.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException("Milestone", id);

        _uow.Milestones.Remove(milestone);

        await WriteAuditLogsAsync(milestone.MilestoneId, userId, "Deleted",
            new List<(string, string?, string?)> { ("IsDeleted", "false", "true") });

        await _uow.SaveChangesAsync();
    }

    public async Task<byte[]> ExportToJsonAsync(int taskId, string userId)
    {
        var milestones = await GetByTaskAsync(taskId, userId);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        };

        return JsonSerializer.SerializeToUtf8Bytes(milestones, options);
    }

    public async Task<byte[]> ExportToXmlAsync(int taskId, string userId)
    {
        var milestones = await GetByTaskAsync(taskId, userId);

        var serializer = new XmlSerializer(typeof(List<MilestoneResponseDto>));
        using var ms = new MemoryStream();
        serializer.Serialize(ms, milestones);
        return ms.ToArray();
    }

    public async Task<byte[]> ExportToICalAsync(int taskId, string userId)
    {
        var milestones = await GetByTaskAsync(taskId, userId);
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//TaskMasterPro//TaskMasterPro 1.0//EN");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:PUBLISH");

        foreach (var milestone in milestones)
        {
            sb.AppendLine("BEGIN:VTODO");
            sb.AppendLine($"UID:milestone-{milestone.MilestoneId}@taskmasterpro.io");
            sb.AppendLine($"DTSTAMP:{milestone.CreatedAt:yyyyMMddTHHmmssZ}");
            sb.AppendLine($"CREATED:{milestone.CreatedAt:yyyyMMddTHHmmssZ}");
            sb.AppendLine($"LAST-MODIFIED:{milestone.UpdatedAt:yyyyMMddTHHmmssZ}");
            sb.AppendLine($"SUMMARY:{EscapeICalValue(milestone.Title)}");
            sb.AppendLine($"DESCRIPTION:{EscapeICalValue(milestone.Description)}");
            sb.AppendLine($"DUE:{milestone.TargetDate:yyyyMMddTHHmmssZ}");

            var status = milestone.Status == MilestoneStatus.Completed ? "COMPLETED" :
                         milestone.Status == MilestoneStatus.Overdue ? "IN-PROCESS" : "NEEDS-ACTION";
            sb.AppendLine($"STATUS:{status}");

            sb.AppendLine("END:VTODO");
        }

        sb.AppendLine("END:VCALENDAR");
        return System.Text.Encoding.UTF8.GetBytes(sb.ToString());
    }

    private string SummarizeChanges(IReadOnlyCollection<(string FieldName, string? OldValue, string? NewValue)> changes)
    {
        var parts = new List<string>();
        foreach (var (field, old, @new) in changes)
            parts.Add($"{field}: {old} → {@new}");
        return string.Join(", ", parts);
    }

    private string EscapeICalValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        return value.Replace("\\", "\\\\")
                    .Replace("\r\n", "\\n")
                    .Replace("\n", "\\n")
                    .Replace(",", "\\,")
                    .Replace(";", "\\;");
    }

    private async Task WriteAuditLogsAsync(
        int milestoneId,
        string userId,
        string action,
        IReadOnlyCollection<(string FieldName, string? OldValue, string? NewValue)> changes)
    {
        foreach (var (fieldName, oldValue, newValue) in changes)
        {
            var log = new MilestoneAuditLog
            {
                MilestoneId = milestoneId,
                Action = action,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedByUserId = userId,
                ChangedAt = DateTime.UtcNow
            };
            await _uow.MilestoneAuditLogs.AddAsync(log);
        }
    }
}



