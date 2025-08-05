using AutoMapper;
using Core.DTOs.Cost.Budgets;
using Core.DTOs.Cost.BudgetItems;
using Core.DTOs.Cost.BudgetRevisions;
using Domain.Entities.Cost.Budget;

namespace Application.Mappings.Cost
{
    /// <summary>
    /// AutoMapper profile for Budget entity mappings
    /// </summary>
    public class BudgetMappingProfile : Profile
    {
        public BudgetMappingProfile()
        {
            // Budget to BudgetDto
            CreateMap<Budget, BudgetDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.TotalBudget, opt => opt.MapFrom(src => src.TotalBudget));

            // Budget to BudgetDetailDto
            CreateMap<Budget, BudgetDetailDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.TotalBudget, opt => opt.MapFrom(src => src.TotalBudget))
                .ForMember(dest => dest.AllocatedAmount, opt => opt.MapFrom(src => src.AllocatedAmount))
                .ForMember(dest => dest.UnallocatedAmount, opt => opt.MapFrom(src => src.UnallocatedAmount))
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.BudgetItems.Count(bi => !bi.IsDeleted)))
                .ForMember(dest => dest.BudgetItems, opt => opt.Ignore()); // Mapped separately in service

            // CreateBudgetDto to Budget - handled manually in service
            CreateMap<CreateBudgetDto, Budget>()
                .ConstructUsing((src, ctx) => 
                    throw new NotImplementedException("Use factory method in service"));

            // UpdateBudgetDto to Budget - handled manually in service
            CreateMap<UpdateBudgetDto, Budget>()
                .ForMember(dest => dest.Name, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.Description, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.Currency, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.ExchangeRate, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.ContingencyPercentage, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.ManagementReservePercentage, opt => opt.Ignore()); // Updated via method

            // BudgetItem mappings
            CreateMap<BudgetItem, BudgetItemDto>()
                .ForMember(dest => dest.ControlAccountCode, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.WorkPackageCode, opt => opt.Ignore()); // Set in service

            CreateMap<CreateBudgetItemDto, BudgetItem>()
                .ConstructUsing((src, ctx) => 
                    throw new NotImplementedException("Use factory method in service"));

            // BudgetRevision mappings
            CreateMap<BudgetRevision, BudgetRevisionDto>()
                .ForMember(dest => dest.ChangeAmount, opt => opt.MapFrom(src => src.ChangeAmount));

            CreateMap<CreateBudgetRevisionDto, BudgetRevision>()
                .ConstructUsing((src, ctx) => 
                    throw new NotImplementedException("Use factory method in service"));
        }
    }
}