using AutoMapper;
using Core.DTOs.Progress.Milestones;
using Domain.Entities.Progress;
using System.Text.Json;

namespace Application.Mappings.Progress;

public class MilestoneProfile : Profile
{
    public MilestoneProfile()
    {
        CreateMap<Milestone, MilestoneDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.PhaseName, opt => opt.MapFrom(src => src.Phase != null ? src.Phase.Name : null))
            .ForMember(dest => dest.WorkPackageName, opt => opt.MapFrom(src => string.Empty)) // Would need to query work package
            .ForMember(dest => dest.PredecessorMilestones, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.PredecessorMilestones) ? null : JsonSerializer.Deserialize<string[]>(src.PredecessorMilestones)))
            .ForMember(dest => dest.SuccessorMilestones, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.SuccessorMilestones) ? null : JsonSerializer.Deserialize<string[]>(src.SuccessorMilestones)));

        CreateMap<CreateMilestoneDto, Milestone>()
            .ConstructUsing(src => new Milestone(
                src.MilestoneCode,
                src.Name,
                src.ProjectId,
                src.Type,
                src.PlannedDate,
                src.IsCritical,
                src.IsContractual))
            .ForMember(dest => dest.Description, opt => opt.Ignore())
            .ForMember(dest => dest.PhaseId, opt => opt.Ignore())
            .ForMember(dest => dest.WorkPackageId, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentAmount, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentCurrency, opt => opt.Ignore())
            .ForMember(dest => dest.CompletionCriteria, opt => opt.Ignore())
            .ForMember(dest => dest.AcceptanceCriteria, opt => opt.Ignore())
            .ForMember(dest => dest.PredecessorMilestones, opt => opt.Ignore())
            .ForMember(dest => dest.SuccessorMilestones, opt => opt.Ignore());

        CreateMap<UpdateMilestoneDto, Milestone>()
            .ForAllMembers(opt => opt.Ignore());
    }
}