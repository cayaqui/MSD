using AutoMapper;
using Core.DTOs.Organization.Contractor;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for Contractor entity mappings
    /// </summary>
    public class ContractorMappingProfile : Profile
    {
        public ContractorMappingProfile()
        {
            // Entity to DTO mapping
            CreateMap<Contractor, ContractorDto>()
                .ForMember(d => d.IsInsuranceExpired, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.CanBeAwarded, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.TotalCommitments, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.OpenCommitments, opt => opt.Ignore()); // Set manually in service

            // Create/Update DTO to Entity mappings are handled in the service
            // since entity creation uses constructor and domain methods
        }
    }
}