using AutoMapper;
using Core.DTOs.Organization.Currency;
using Domain.Entities.Organization.Core;

namespace Application.Mappings.Organization
{
    /// <summary>
    /// AutoMapper profile for Currency entity mappings
    /// </summary>
    public class CurrencyMappingProfile : Profile
    {
        public CurrencyMappingProfile()
        {
            // Entity to DTO mapping
            CreateMap<Currency, CurrencyDto>()
                .ForMember(d => d.IsExchangeRateStale, opt => opt.Ignore()); // Set manually in service

            // Create/Update DTO to Entity mappings are handled in the service
            // since entity creation uses constructor and domain methods
        }
    }
}