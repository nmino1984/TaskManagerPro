using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.CalendarEvent;
using MyApp.Application.Exceptions;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Application.Services;

public class CalendarEventService : ICalendarEventService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CalendarEventService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CalendarEventResponseDto>> GetByTaskAsync(int taskId)
    {
        var events = await _db.CalendarEvents
            .Where(e => e.TaskId == taskId)
            .ToListAsync();

        return _mapper.Map<List<CalendarEventResponseDto>>(events);
    }

    public async Task<CalendarEventResponseDto> GetByIdAsync(int id)
    {
        var ev = await _db.CalendarEvents.FindAsync(id)
            ?? throw new NotFoundException("CalendarEvent", id);

        return _mapper.Map<CalendarEventResponseDto>(ev);
    }

    public async Task<CalendarEventResponseDto> CreateAsync(CalendarEventCreateDto dto)
    {
        var taskExists = await _db.MyTasks.AnyAsync(t => t.MyTaskId == dto.TaskId);
        if (!taskExists)
            throw new NotFoundException("MyTask", dto.TaskId);

        var ev = _mapper.Map<CalendarEvent>(dto);

        _db.CalendarEvents.Add(ev);
        await _db.SaveChangesAsync();

        return _mapper.Map<CalendarEventResponseDto>(ev);
    }

    public async Task<CalendarEventResponseDto> UpdateAsync(int id, CalendarEventUpdateDto dto)
    {
        var ev = await _db.CalendarEvents.FindAsync(id)
            ?? throw new NotFoundException("CalendarEvent", id);

        _mapper.Map(dto, ev);

        await _db.SaveChangesAsync();

        return _mapper.Map<CalendarEventResponseDto>(ev);
    }

    public async Task DeleteAsync(int id)
    {
        var ev = await _db.CalendarEvents.FindAsync(id)
            ?? throw new NotFoundException("CalendarEvent", id);

        _db.CalendarEvents.Remove(ev);
        await _db.SaveChangesAsync();
    }
}
