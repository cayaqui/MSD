using AutoMapper;
using Core.DTOs.Organization.OBSNode;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for OBSNode entity mappings
    /// </summary>
    public class OBSNodeMappingProfile : Profile
    {
        public OBSNodeMappingProfile()
        {
            // Entity to DTO mapping
            CreateMap<OBSNode, OBSNodeDto>()
                .ForMember(d => d.UtilizationRate, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.HasChildren, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.MemberCount, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.ParentName, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.ProjectName, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.ProjectCode, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.ManagerName, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.ManagerEmail, opt => opt.Ignore()) // Set manually in service
                .ForMember(d => d.Children, opt => opt.Ignore()); // Handled separately for hierarchy

            // Entity to OBSNodeHierarchyDto mapping
            CreateMap<OBSNode, OBSNodeHierarchyDto>()
                .IncludeBase<OBSNode, OBSNodeDto>()
                .ForMember(d => d.Children, opt => opt.Ignore()); // Built manually in service

            // Create/Update DTO to Entity mappings are handled in the service
            // since entity creation uses constructor and domain methods
        }
    }
}