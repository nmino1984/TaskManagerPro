using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.Milestone;
using MyApp.Application.Exceptions;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using MyApp.Domain.Enums;
using MyApp.Infrastructure.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MyApp.Application.Services;

public class MilestoneService : IMilestoneService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public MilestoneService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<MilestoneResponseDto>> GetByTaskAsync(int taskId)
    {
        var milestones = await _db.Milestones
            .Where(m => m.TaskId == taskId)
            .ToListAsync();

        return _mapper.Map<List<MilestoneResponseDto>>(milestones);
    }

    public async Task<MilestoneResponseDto> GetByIdAsync(int id)
    {
        var milestone = await _db.Milestones.FindAsync(id)
            ?? throw new NotFoundException("Milestone", id);

        return _mapper.Map<MilestoneResponseDto>(milestone);
    }

    public async Task<MilestoneResponseDto> CreateAsync(MilestoneCreateDto dto)
    {
        var taskExists = await _db.MyTasks.AnyAsync(t => t.MyTaskId == dto.TaskId);
        if (!taskExists)
            throw new NotFoundException("MyTask", dto.TaskId);

        var milestone = _mapper.Map<Milestone>(dto);
        milestone.CreatedAt = DateTime.UtcNow;
        milestone.UpdatedAt = DateTime.UtcNow;

        _db.Milestones.Add(milestone);
        await _db.SaveChangesAsync();

        return _mapper.Map<MilestoneResponseDto>(milestone);
    }

    public async Task<MilestoneResponseDto> UpdateAsync(int id, MilestoneUpdateDto dto)
    {
        var milestone = await _db.Milestones.FindAsync(id)
            ?? throw new NotFoundException("Milestone", id);

        _mapper.Map(dto, milestone);
        milestone.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return _mapper.Map<MilestoneResponseDto>(milestone);
    }

    public async Task DeleteAsync(int id)
    {
        var milestone = await _db.Milestones.FindAsync(id)
            ?? throw new NotFoundException("Milestone", id);

        _db.Milestones.Remove(milestone);
        await _db.SaveChangesAsync();
    }

    public async Task<byte[]> ExportToJsonAsync(int taskId)
    {
        var milestones = await GetByTaskAsync(taskId);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        };

        return JsonSerializer.SerializeToUtf8Bytes(milestones, options);
    }

    public async Task<byte[]> ExportToXmlAsync(int taskId)
    {
        var milestones = await GetByTaskAsync(taskId);

        var serializer = new XmlSerializer(typeof(List<MilestoneResponseDto>));
        using var ms = new MemoryStream();
        serializer.Serialize(ms, milestones);
        return ms.ToArray();
    }

    public async Task<byte[]> ExportToICalAsync(int taskId)
    {
        var milestones = await GetByTaskAsync(taskId);
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

    private string EscapeICalValue(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        return value.Replace("\\", "\\\\")
                    .Replace("\r\n", "\\n")
                    .Replace("\n", "\\n")
                    .Replace(",", "\\,")
                    .Replace(";", "\\;");
    }
}
