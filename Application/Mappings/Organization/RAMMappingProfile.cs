using AutoMapper;
using Core.DTOs.Organization.RAM;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for RAM entity mappings
    /// </summary>
    public class RAMMappingProfile : Profile
    {
        public RAMMappingProfile()
        {
            // Entity to DTO
            CreateMap<RAM, RAMDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.WBSCode, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.WBSName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.OBSCode, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.OBSName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.ControlAccountCode, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.ResponsibilityDescription, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.AllocationPercentage, opt => opt.Ignore()) // Mapped in service from AllocatedPercentage
                .ForMember(dest => dest.PlannedManHours, opt => opt.Ignore()) // Mapped in service from AllocatedHours
                .ForMember(dest => dest.PlannedCost, opt => opt.Ignore()); // Not in entity

            // CreateDto to Entity - handled manually in service
            CreateMap<CreateRAMDto, RAM>()
                .ConstructUsing((src, ctx) => 
                    throw new NotImplementedException("Use constructor in service"));

            // UpdateDto to Entity - handled manually in service  
            CreateMap<UpdateRAMDto, RAM>()
                .ForMember(dest => dest.ResponsibilityType, opt => opt.Ignore()) // Set via constructor
                .ForMember(dest => dest.AllocatedHours, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.AllocatedPercentage, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.StartDate, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.EndDate, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.ControlAccountId, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.Notes, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}