using AutoMapper;
using Core.DTOs.Organization.Phase;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for Phase entity mappings
    /// </summary>
    public class PhaseMappingProfile : Profile
    {
        public PhaseMappingProfile()
        {
            // Entity to DTO mapping
            CreateMap<Phase, PhaseDto>()
                .ForMember(d => d.IsOverBudget, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.IsDelayed, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.ProjectName, opt => opt.Ignore()); // Set manually in service

            // Entity to PhaseDetailDto mapping
            CreateMap<Phase, PhaseDetailDto>()
                .IncludeBase<Phase, PhaseDto>()
                .ForMember(d => d.WBSElementCount, opt => opt.Ignore())
                .ForMember(d => d.ControlAccountCount, opt => opt.Ignore())
                .ForMember(d => d.MilestoneCount, opt => opt.Ignore())
                .ForMember(d => d.CompletedMilestones, opt => opt.Ignore())
                .ForMember(d => d.PlanningPackageCount, opt => opt.Ignore())
                .ForMember(d => d.BudgetVariance, opt => opt.Ignore())
                .ForMember(d => d.BudgetVariancePercentage, opt => opt.Ignore())
                .ForMember(d => d.CostPerformanceIndex, opt => opt.Ignore())
                .ForMember(d => d.PlannedDuration, opt => opt.Ignore())
                .ForMember(d => d.ActualDuration, opt => opt.Ignore())
                .ForMember(d => d.DaysRemaining, opt => opt.Ignore())
                .ForMember(d => d.SchedulePerformanceIndex, opt => opt.Ignore())
                .ForMember(d => d.AssignedResources, opt => opt.Ignore())
                .ForMember(d => d.PlannedEffort, opt => opt.Ignore())
                .ForMember(d => d.ActualEffort, opt => opt.Ignore());

            // Create/Update DTO to Entity mappings are handled in the service
            // since entity creation uses constructor and domain methods
        }
    }
}