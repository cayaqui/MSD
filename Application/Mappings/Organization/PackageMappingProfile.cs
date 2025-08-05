using AutoMapper;
using Core.DTOs.Organization.Package;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for Package entity mappings
    /// </summary>
    public class PackageMappingProfile : Profile
    {
        public PackageMappingProfile()
        {
            // Entity to DTO mapping
            CreateMap<Package, PackageDto>()
                .ForMember(d => d.IsOverdue, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.DaysOverdue, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.ScheduleVariance, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.PhaseName, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.WBSElementName, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.ContractorName, opt => opt.Ignore()); // Set manually in service

            // Entity to PackageWithDisciplinesDto mapping
            CreateMap<Package, PackageWithDisciplinesDto>()
                .IncludeBase<Package, PackageDto>()
                .ForMember(d => d.Disciplines, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.TotalDisciplines, opt => opt.Ignore())
                .ForMember(d => d.TotalEstimatedHours, opt => opt.Ignore())
                .ForMember(d => d.TotalActualHours, opt => opt.Ignore())
                .ForMember(d => d.TotalEstimatedCost, opt => opt.Ignore())
                .ForMember(d => d.TotalActualCost, opt => opt.Ignore());

            // Create/Update DTO to Entity mappings are handled in the service
            // since entity creation uses constructor and domain methods
        }
    }
}