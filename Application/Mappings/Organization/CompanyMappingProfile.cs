using AutoMapper;
using Core.DTOs.Organization.Company;
using Core.DTOs.Organization.Operation;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for Company entity mappings
    /// </summary>
    public class CompanyMappingProfile : Profile
    {
        public CompanyMappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Company, CompanyDto>()
                .ForMember(d => d.HasLogo, opt => opt.MapFrom(s => s.Logo != null && s.Logo.Length > 0));

            CreateMap<Company, CompanyWithOperationsDto>()
                .ForMember(d => d.HasLogo, opt => opt.MapFrom(s => s.Logo != null && s.Logo.Length > 0))
                .ForMember(d => d.Operations, opt => opt.MapFrom(s => s.Operations));

            // Operation mapping
            CreateMap<Operation, OperationDto>();

            // Create/Update DTO to Entity mappings are handled in the service
            // since entity creation uses constructor and domain methods
        }
    }
}