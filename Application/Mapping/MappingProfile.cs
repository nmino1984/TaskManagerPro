using AutoMapper;
using MyApp.Application.DTOs;
using MyApp.Application.DTOs.CalendarEvent;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.DTOs.SubTask;
using MyApp.Domain.Entities;

namespace MyApp.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //
        // ─────────────────────────────────────────────
        //   MYTASK
        // ─────────────────────────────────────────────
        //

        CreateMap<MyTaskCreateDto, MyTask>();

        CreateMap<MyTaskUpdateDto, MyTask>()
            .ForMember(dest => dest.MyTaskId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<MyTask, MyTaskResponseDto>()
            .ForMember(dest => dest.SubTasks, opt => opt.MapFrom(src => src.SubTasks))
            .ForMember(dest => dest.CalendarEvents, opt => opt.MapFrom(src => src.CalendarEvents));


        //
        // ─────────────────────────────────────────────
        //   SUBTASK
        // ─────────────────────────────────────────────
        //

        CreateMap<SubTaskCreateDto, SubTask>();

        CreateMap<SubTaskUpdateDto, SubTask>()
            .ForMember(dest => dest.SubTaskId, opt => opt.Ignore())
            .ForMember(dest => dest.TaskId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<SubTask, SubTaskResponseDto>();


        //
        // ─────────────────────────────────────────────
        //   CALENDAR EVENT
        // ─────────────────────────────────────────────
        //

        CreateMap<CalendarEventCreateDto, CalendarEvent>();

        CreateMap<CalendarEventUpdateDto, CalendarEvent>()
            .ForMember(dest => dest.CalendarEventId, opt => opt.Ignore())
            .ForMember(dest => dest.TaskId, opt => opt.Ignore());

        CreateMap<CalendarEvent, CalendarEventResponseDto>();
    }
}
