using AutoMapper;
using Core.DTOs.Organization.Operation;
using Core.DTOs.Organization.Project;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for Operation entity mappings
    /// </summary>
    public class OperationMappingProfile : Profile
    {
        public OperationMappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Operation, OperationDto>()
                .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company != null ? s.Company.Name : string.Empty));

            CreateMap<Operation, OperationWithProjectsDto>()
                .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company != null ? s.Company.Name : string.Empty))
                .ForMember(d => d.Projects, opt => opt.MapFrom(s => s.Projects));

            // Project mapping for OperationWithProjectsDto
            CreateMap<Project, ProjectSummaryDto>();

            // Create/Update DTO to Entity mappings are handled in the service
            // since entity creation uses constructor and domain methods
        }
    }
}