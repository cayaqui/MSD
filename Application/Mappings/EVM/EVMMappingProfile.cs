using AutoMapper;
using Core.DTOs.EVM;
using Domain.Entities.Cost.EVM;

namespace Application.Mappings.EVM;

/// <summary>
/// AutoMapper profile for EVM-related mappings
/// </summary>
public class EVMMappingProfile : Profile
{
    public EVMMappingProfile()
    {
        // EVMRecord mappings
        CreateMap<EVMRecord, EVMRecordDto>()
            .ForMember(dest => dest.ControlAccountCode, 
                opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Code : string.Empty))
            .ForMember(dest => dest.ControlAccountName, 
                opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Name : string.Empty))
            .ForMember(dest => dest.CV, opt => opt.MapFrom(src => src.CV))
            .ForMember(dest => dest.SV, opt => opt.MapFrom(src => src.SV))
            .ForMember(dest => dest.CPI, opt => opt.MapFrom(src => src.CPI))
            .ForMember(dest => dest.SPI, opt => opt.MapFrom(src => src.SPI));

        CreateMap<EVMRecord, EVMRecordDetailDto>()
            .ForMember(dest => dest.ControlAccountCode, 
                opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Code : string.Empty))
            .ForMember(dest => dest.ControlAccountName, 
                opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Name : string.Empty))
            .ForMember(dest => dest.CV, opt => opt.MapFrom(src => src.CV))
            .ForMember(dest => dest.SV, opt => opt.MapFrom(src => src.SV))
            .ForMember(dest => dest.CPI, opt => opt.MapFrom(src => src.CPI))
            .ForMember(dest => dest.SPI, opt => opt.MapFrom(src => src.SPI))
            .ForMember(dest => dest.VAC, opt => opt.MapFrom(src => src.VAC))
            .ForMember(dest => dest.TCPI, opt => opt.MapFrom(src => src.TCPI))
            .ForMember(dest => dest.PercentComplete, 
                opt => opt.MapFrom(src => src.ActualPercentComplete ?? 0))
            .ForMember(dest => dest.PercentSpent, 
                opt => opt.MapFrom(src => src.BAC > 0 ? src.AC / src.BAC * 100 : 0));
    }
}