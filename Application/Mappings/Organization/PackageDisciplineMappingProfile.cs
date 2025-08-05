using AutoMapper;
using Core.DTOs.Organization.Package;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for PackageDiscipline entity mappings
    /// </summary>
    public class PackageDisciplineMappingProfile : Profile
    {
        public PackageDisciplineMappingProfile()
        {
            // Entity to DTO
            CreateMap<PackageDiscipline, PackageDisciplineDto>()
                .ForMember(dest => dest.PackageCode, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.PackageName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.DisciplineCode, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.DisciplineName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.LeadEngineerName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.ProductivityRate, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.CostVariance, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.HoursVariance, opt => opt.Ignore()); // Set in service

            // CreateDto to Entity - handled manually in service
            CreateMap<CreatePackageDisciplineDto, PackageDiscipline>()
                .ConstructUsing((src, ctx) => 
                    throw new NotImplementedException("Use constructor in service"));

            // UpdateDto to Entity - handled manually in service  
            CreateMap<UpdatePackageDisciplineDto, PackageDiscipline>()
                .ForMember(dest => dest.EstimatedHours, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.EstimatedCost, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.ActualHours, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.ActualCost, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.ProgressPercentage, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.IsLeadDiscipline, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.LeadEngineerId, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.Notes, opt => opt.Ignore()); // Updated via method
        }
    }
}