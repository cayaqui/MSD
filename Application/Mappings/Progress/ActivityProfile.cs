using AutoMapper;
using Core.DTOs.Progress.Activities;
using Domain.Entities.Progress;
using System.Text.Json;

namespace Application.Mappings.Progress;

public class ActivityProfile : Profile
{
    public ActivityProfile()
    {
        CreateMap<Activity, ActivityDto>()
            .ForMember(dest => dest.WBSCode, opt => opt.MapFrom(src => src.WBSElement != null ? src.WBSElement.Code : string.Empty))
            .ForMember(dest => dest.WBSName, opt => opt.MapFrom(src => src.WBSElement != null ? src.WBSElement.Name : string.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.WBSElement != null && src.WBSElement.Project != null ? src.WBSElement.Project.Name : string.Empty))
            .ForMember(dest => dest.PredecessorActivities, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.PredecessorActivities) ? null : JsonSerializer.Deserialize<string[]>(src.PredecessorActivities)))
            .ForMember(dest => dest.SuccessorActivities, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.SuccessorActivities) ? null : JsonSerializer.Deserialize<string[]>(src.SuccessorActivities)))
            .ForMember(dest => dest.IsOnSchedule, opt => opt.MapFrom(src => src.IsOnSchedule()))
            .ForMember(dest => dest.DelayDays, opt => opt.MapFrom(src => src.GetDelayDays()))
            .ForMember(dest => dest.ProductivityRate, opt => opt.MapFrom(src => src.GetProductivityRate()));

        CreateMap<Activity, CriticalActivityDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateActivityDto, Activity>()
            .ConstructUsing(src => new Activity(
                src.WBSElementId,
                src.ActivityCode,
                src.Name,
                src.PlannedStartDate,
                src.PlannedEndDate,
                src.PlannedHours))
            .ForMember(dest => dest.PredecessorActivities, opt => opt.Ignore())
            .ForMember(dest => dest.SuccessorActivities, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.Ignore())
            .ForMember(dest => dest.ResourceRate, opt => opt.Ignore());

        CreateMap<UpdateActivityDto, Activity>()
            .ForAllMembers(opt => opt.Ignore());
    }
}