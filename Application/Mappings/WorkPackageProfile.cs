using AutoMapper;
using Core.DTOs.WorkPackages;
using Domain.Entities.Progress;

namespace Application.Mappings;

/// <summary>
/// AutoMapper profile for Work Package-related mappings
/// </summary>
public class WorkPackageProfile : Profile
{
    public WorkPackageProfile()
    {
        // WorkPackageProgress mappings
        CreateMap<WorkPackageProgress, WorkPackageProgressDto>()
            .ForMember(dest => dest.ProgressPercentage, 
                opt => opt.MapFrom(src => src.CurrentProgress))
            .ForMember(dest => dest.ActualCost, 
                opt => opt.MapFrom(src => src.CurrentActualCost))
            .ForMember(dest => dest.ReportedBy, 
                opt => opt.MapFrom(src => src.CreatedBy ?? "Unknown"));

        // Activity mappings
        CreateMap<Activity, ActivityDto>();

        CreateMap<CreateActivityDto, Activity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WBSElement, opt => opt.Ignore())
            .ForMember(dest => dest.WorkPackageDetails, opt => opt.Ignore())
            .ForMember(dest => dest.Resources, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
    }
}