using AutoMapper;
using Core.DTOs.Cost.CostItems;
using Core.DTOs.Cost.PlanningPackages;
using Domain.Entities.Cost.Control;
using Domain.Entities.WBS;

namespace Application.Mappings.Cost
{
    /// <summary>
    /// AutoMapper profile for Cost entity mappings
    /// </summary>
    public class CostMappingProfile : Profile
    {
        public CostMappingProfile()
        {
            // CostItem mappings
            CreateMap<CostItem, CostItemDto>()
                .ForMember(dest => dest.ProjectName, 
                    opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.WorkPackageCode, 
                    opt => opt.MapFrom(src => src.WBSElement != null ? src.WBSElement.Code : null))
                .ForMember(dest => dest.ControlAccountCode, 
                    opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Code : null));

            CreateMap<CostItem, CostItemDetailDto>()
                .ForMember(dest => dest.ProjectName, 
                    opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.WorkPackageId,
                    opt => opt.MapFrom(src => src.WBSElementId))
                .ForMember(dest => dest.WorkPackageCode, 
                    opt => opt.MapFrom(src => src.WBSElement != null ? src.WBSElement.Code : null))
                .ForMember(dest => dest.ControlAccountCode, 
                    opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Code : null));

            CreateMap<CreateCostItemDto, CostItem>()
                .ConstructUsing((src, ctx) =>
                    new CostItem(
                        src.ProjectId,
                        src.ItemCode,
                        src.Description,
                        src.Type,
                        src.Category,
                        src.PlannedCost));

            // PlanningPackage mappings
            CreateMap<PlanningPackage, PlanningPackageDto>()
                .ForMember(dest => dest.ControlAccountCode, 
                    opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Code : string.Empty))
                .ForMember(dest => dest.ControlAccountName, 
                    opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Name : string.Empty))
                .ForMember(dest => dest.Budget, 
                    opt => opt.MapFrom(src => src.EstimatedBudget))
                .ForMember(dest => dest.Currency, 
                    opt => opt.MapFrom(src => "USD")); // Default currency

            CreateMap<CreatePlanningPackageDto, PlanningPackage>()
                .ConstructUsing((src, ctx) =>
                    throw new NotImplementedException("Use factory method in service"));
        }
    }
}