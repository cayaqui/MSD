using AutoMapper;
using Core.DTOs.Organization.Discipline;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for Discipline entity mappings
    /// </summary>
    public class DisciplineMappingProfile : Profile
    {
        public DisciplineMappingProfile()
        {
            // Entity to DTO
            CreateMap<Discipline, DisciplineDto>();

            // CreateDto to Entity - handled manually in service
            CreateMap<CreateDisciplineDto, Discipline>()
                .ConstructUsing(src => new Discipline(
                    src.Code,
                    src.Name,
                    src.ColorHex,
                    src.Order,
                    src.IsEngineering))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Icon, opt => opt.Ignore()); // Set via method

            // UpdateDto to Entity - handled manually in service  
            CreateMap<UpdateDisciplineDto, Discipline>()
                .ForMember(dest => dest.Code, opt => opt.Ignore()) // Code cannot be changed
                .ForMember(dest => dest.Name, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.Description, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.Order, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.ColorHex, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.Icon, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.IsEngineering, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.IsManagement, opt => opt.Ignore()); // Updated via method
        }
    }
}