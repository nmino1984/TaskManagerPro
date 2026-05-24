using AutoMapper;
using MyApp.Application.DTOs;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.DTOs.SubTask;
using MyApp.Application.DTOs.Milestone;
using MyApp.Application.DTOs.Notification;
using MyApp.Application.DTOs.User;
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
            .ForMember(dest => dest.Milestones, opt => opt.MapFrom(src => src.Milestones))
            .ForMember(dest => dest.AssignedToUsername, opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.Username : null));


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
        //   MILESTONE
        // ─────────────────────────────────────────────
        //

        CreateMap<MilestoneCreateDto, Milestone>();

        CreateMap<MilestoneUpdateDto, Milestone>()
            .ForMember(dest => dest.MilestoneId, opt => opt.Ignore())
            .ForMember(dest => dest.TaskId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<Milestone, MilestoneResponseDto>();


        //
        // ─────────────────────────────────────────────
        //   NOTIFICATION
        // ─────────────────────────────────────────────
        //

        CreateMap<Notification, NotificationResponseDto>();


        //
        // ─────────────────────────────────────────────
        //   USER
        // ─────────────────────────────────────────────
        //

        CreateMap<User, UserDto>();
    }
}
