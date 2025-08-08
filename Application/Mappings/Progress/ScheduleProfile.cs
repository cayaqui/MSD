using AutoMapper;
using Core.DTOs.Progress.Schedules;
using Domain.Entities.Progress;

namespace Application.Mappings.Progress;

public class ScheduleProfile : Profile
{
    public ScheduleProfile()
    {
        CreateMap<ScheduleVersion, ScheduleVersionDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.ActivityCount, opt => opt.MapFrom(src => src.Activities.Count))
            .ForMember(dest => dest.MilestoneCount, opt => opt.MapFrom(src => 0)); // Would need to query milestones

        CreateMap<CreateScheduleVersionDto, ScheduleVersion>()
            .ConstructUsing(src => new ScheduleVersion(
                src.ProjectId,
                src.Version,
                src.Name,
                src.PlannedStartDate,
                src.PlannedEndDate))
            .ForMember(dest => dest.Description, opt => opt.Ignore())
            .ForMember(dest => dest.DataDate, opt => opt.Ignore())
            .ForMember(dest => dest.SourceSystem, opt => opt.Ignore());

        CreateMap<UpdateScheduleVersionDto, ScheduleVersion>()
            .ForAllMembers(opt => opt.Ignore());
    }
}