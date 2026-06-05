using AutoMapper;
using TaskManagerPro.Application.DTOs.AuditLog;
using TaskManagerPro.Application.DTOs.Milestone;
using TaskManagerPro.Application.DTOs.MyTask;
using TaskManagerPro.Application.DTOs.Notification;
using TaskManagerPro.Application.DTOs.SubTask;
using TaskManagerPro.Application.DTOs.TaskComment;
using TaskManagerPro.Application.DTOs.User;
using TaskManagerPro.Domain.Entities;
namespace TaskManagerPro.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //
        // ---------------------------------------------
        //   MYTASK
        // ---------------------------------------------
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
        // ---------------------------------------------
        //   SUBTASK
        // ---------------------------------------------
        //

        CreateMap<SubTaskCreateDto, SubTask>();

        CreateMap<SubTaskUpdateDto, SubTask>()
            .ForMember(dest => dest.SubTaskId, opt => opt.Ignore())
            .ForMember(dest => dest.TaskId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<SubTask, SubTaskResponseDto>();


        //
        // ---------------------------------------------
        //   MILESTONE
        // ---------------------------------------------
        //

        CreateMap<MilestoneCreateDto, Milestone>();

        CreateMap<MilestoneUpdateDto, Milestone>()
            .ForMember(dest => dest.MilestoneId, opt => opt.Ignore())
            .ForMember(dest => dest.TaskId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<Milestone, MilestoneResponseDto>();


        //
        // ---------------------------------------------
        //   NOTIFICATION
        // ---------------------------------------------
        //

        CreateMap<Notification, NotificationResponseDto>();


        //
        // ---------------------------------------------
        //   USER
        // ---------------------------------------------
        //

        CreateMap<User, UserDto>();


        //
        // ---------------------------------------------
        //   AUDIT LOG
        // ---------------------------------------------
        //

        CreateMap<TaskAuditLog, TaskAuditLogResponseDto>()
            .ForMember(dest => dest.ChangedByUsername,
                       opt => opt.MapFrom(src => src.ChangedByUser != null ? src.ChangedByUser.Username : null));

        CreateMap<SubTaskAuditLog, SubTaskAuditLogResponseDto>()
            .ForMember(dest => dest.ChangedByUsername,
                       opt => opt.MapFrom(src => src.ChangedByUser != null ? src.ChangedByUser.Username : null));

        CreateMap<MilestoneAuditLog, MilestoneAuditLogResponseDto>()
            .ForMember(dest => dest.ChangedByUsername,
                       opt => opt.MapFrom(src => src.ChangedByUser != null ? src.ChangedByUser.Username : null));


        //
        // ---------------------------------------------
        //   TASK COMMENT
        // ---------------------------------------------
        //

        CreateMap<TaskComment, TaskCommentResponseDto>()
            .ForMember(dest => dest.CreatedByUsername,
                       opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Username : null))
            .ForMember(dest => dest.Replies,
                       opt => opt.MapFrom(src => src.Replies));
    }
}

