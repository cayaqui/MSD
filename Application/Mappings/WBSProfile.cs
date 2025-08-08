using AutoMapper;
using Core.DTOs.Projects.WBSElement;
using Core.DTOs.Projects.WorkPackageDetails;
using Core.DTOs.Cost.PlanningPackages;
using Domain.Entities.WBS;
using Domain.Entities.Cost;
using Domain.Entities.Cost.Core;

namespace Application.Mappings;

/// <summary>
/// AutoMapper profile for WBS-related mappings
/// </summary>
public class WBSProfile : Profile
{
    public WBSProfile()
    {
        // WBSElement mappings
        CreateMap<WBSElement, WBSElementDto>()
            .ForMember(dest => dest.ControlAccountCode, 
                opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Code : null))
            .ForMember(dest => dest.ChildrenCount,
                opt => opt.MapFrom(src => src.Children.Count))
            .ForMember(dest => dest.CanHaveChildren,
                opt => opt.MapFrom(src => src.ElementType != Core.Enums.Projects.WBSElementType.WorkPackage && 
                                         src.ElementType != Core.Enums.Projects.WBSElementType.PlanningPackage))
            .ForMember(dest => dest.ProgressPercentage,
                opt => opt.MapFrom(src => src.WorkPackageDetails != null ? src.WorkPackageDetails.ProgressPercentage : (decimal?)null))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.WorkPackageDetails != null ? src.WorkPackageDetails.Status : (Core.Enums.Progress.WorkPackageStatus?)null));

        CreateMap<WBSElement, WBSElementDetailDto>()
            .ForMember(dest => dest.ControlAccountCode,
                opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Code : null))
            .ForMember(dest => dest.ChildrenCount,
                opt => opt.MapFrom(src => src.Children.Count))
            .ForMember(dest => dest.CanHaveChildren,
                opt => opt.MapFrom(src => src.ElementType != Core.Enums.Projects.WBSElementType.WorkPackage && 
                                         src.ElementType != Core.Enums.Projects.WBSElementType.PlanningPackage))
            .ForMember(dest => dest.ProgressPercentage,
                opt => opt.MapFrom(src => src.WorkPackageDetails != null ? src.WorkPackageDetails.ProgressPercentage : (decimal?)null))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.WorkPackageDetails != null ? src.WorkPackageDetails.Status : (Core.Enums.Progress.WorkPackageStatus?)null))
            .ForMember(dest => dest.Dictionary,
                opt => opt.Ignore())
            .ForMember(dest => dest.Children,
                opt => opt.MapFrom(src => src.Children))
            .ForMember(dest => dest.WorkPackageDetails,
                opt => opt.MapFrom(src => src.WorkPackageDetails))
            .ForMember(dest => dest.CBSMappings,
                opt => opt.MapFrom(src => src.CBSMappings))
            .AfterMap((src, dest) =>
            {
                // Map Dictionary after main mapping
                if (src.DeliverableDescription != null || 
                    src.AcceptanceCriteria != null ||
                    src.Assumptions != null ||
                    src.Constraints != null ||
                    src.ExclusionsInclusions != null)
                {
                    dest.Dictionary = new WBSDictionaryDto
                    {
                        WBSElementId = src.Id,
                        DeliverableDescription = src.DeliverableDescription,
                        AcceptanceCriteria = src.AcceptanceCriteria,
                        Assumptions = src.Assumptions,
                        Constraints = src.Constraints,
                        ExclusionsInclusions = src.ExclusionsInclusions
                    };
                }
            });

        // WorkPackageDetails mappings
        CreateMap<WorkPackageDetails, WorkPackageDetailsDto>()
            .ForMember(dest => dest.ResponsibleUserName,
                opt => opt.MapFrom(src => src.ResponsibleUser != null ? src.ResponsibleUser.Name : null))
            .ForMember(dest => dest.PrimaryDisciplineName,
                opt => opt.MapFrom(src => src.PrimaryDiscipline != null ? src.PrimaryDiscipline.Name : null));

        // PlanningPackage mappings
        CreateMap<PlanningPackage, PlanningPackageDto>()
            .ForMember(dest => dest.ControlAccountCode,
                opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Code : null))
            .ForMember(dest => dest.ControlAccountName,
                opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Name : null));

        // WBSCBSMapping mappings
        CreateMap<WBSCBSMapping, WBSCBSMappingDto>()
            .ForMember(dest => dest.CBSCode,
                opt => opt.MapFrom(src => src.CBS != null ? src.CBS.Code : null))
            .ForMember(dest => dest.CBSName,
                opt => opt.MapFrom(src => src.CBS != null ? src.CBS.Name : null));

        // Reverse mappings (if needed)
        CreateMap<CreateWBSElementDto, WBSElement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Children, opt => opt.Ignore())
            .ForMember(dest => dest.Parent, opt => opt.Ignore())
            .ForMember(dest => dest.Project, opt => opt.Ignore())
            .ForMember(dest => dest.ControlAccount, opt => opt.Ignore())
            .ForMember(dest => dest.WorkPackageDetails, opt => opt.Ignore())
            .ForMember(dest => dest.CBSMappings, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}