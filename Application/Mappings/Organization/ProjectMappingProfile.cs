using AutoMapper;
using Core.DTOs.Organization.Project;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for Project entity mappings
    /// </summary>
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Project, ProjectDto>()
                .ForMember(d => d.OperationName, opt => opt.MapFrom(s => s.Operation != null ? s.Operation.Name : string.Empty))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.TeamMemberCount, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.PhaseCount, opt => opt.Ignore()); // Set manually in service

            CreateMap<Project, ProjectSummaryDto>()
                .ForMember(d => d.OperationName, opt => opt.MapFrom(s => s.Operation != null ? s.Operation.Name : string.Empty))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.TeamMemberCount, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.PhaseCount, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.BudgetUtilization, opt => opt.MapFrom(s => s.ActualCost.HasValue && s.TotalBudget > 0 ? (s.ActualCost.Value / s.TotalBudget) * 100 : 0))
                .ForMember(d => d.CostPerformanceIndex, opt => opt.MapFrom(s => 1)) // Would need EVM data
                .ForMember(d => d.SchedulePerformanceIndex, opt => opt.MapFrom(s => 1)) // Would need EVM data
                .ForMember(d => d.DaysRemaining, opt => opt.MapFrom(s => s.Status == Core.Enums.Projects.ProjectStatus.Active ? (s.PlannedEndDate - DateTime.UtcNow).Days : 0))
                .ForMember(d => d.DaysOverdue, opt => opt.MapFrom(s => s.IsDelayed() ? (DateTime.UtcNow - s.PlannedEndDate).Days : 0))
                .ForMember(d => d.IsOverdue, opt => opt.MapFrom(s => s.IsDelayed()))
                .ForMember(d => d.IsOverBudget, opt => opt.MapFrom(s => s.IsOverBudget()))
                .ForMember(d => d.ActiveTeamMembers, opt => opt.MapFrom(s => s.ProjectTeamMembers != null ? s.ProjectTeamMembers.Count(m => m.IsActive) : 0))
                .ForMember(d => d.TeamUtilization, opt => opt.MapFrom(s => 0)) // Would need calculation
                .ForMember(d => d.CompletedPhases, opt => opt.MapFrom(s => s.Phases != null ? s.Phases.Count(p => p.Status == Core.Enums.Projects.PhaseStatus.Completed) : 0))
                .ForMember(d => d.TotalPhases, opt => opt.MapFrom(s => s.Phases != null ? s.Phases.Count : 0))
                .ForMember(d => d.PhaseProgress, opt => opt.MapFrom(s => s.Phases != null && s.Phases.Count > 0 ? (decimal)s.Phases.Count(p => p.Status == Core.Enums.Projects.PhaseStatus.Completed) / s.Phases.Count * 100 : 0));

            // Create/Update DTO to Entity mappings are handled in the service
            // since entity creation uses constructor and domain methods
        }
    }
}