using AutoMapper;
using Core.DTOs.Cost.ControlAccounts;
using Core.Enums.Cost;
using Domain.Entities.Cost.Control;
using Domain.Entities.Cost.EVM;

namespace Application.Mappings.Cost
{
    /// <summary>
    /// AutoMapper profile for Control Account entity mappings
    /// </summary>
    public class ControlAccountMappingProfile : Profile
    {
        public ControlAccountMappingProfile()
        {
            // ControlAccount to ControlAccountDto
            CreateMap<ControlAccount, ControlAccountDto>()
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Control Account"))
                .ForMember(dest => dest.ProjectName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.PhaseName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.CAMUserId, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.CAMName, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.AC, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.EV, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.PV, opt => opt.Ignore()); // Set in service

            // ControlAccount to ControlAccountDetailDto
            CreateMap<ControlAccount, ControlAccountDetailDto>()
                .IncludeBase<ControlAccount, ControlAccountDto>()
                .ForMember(dest => dest.ActualCost, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.EarnedValue, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.PlannedValue, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.CPI, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.SPI, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.WorkPackageCount, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.PlanningPackageCount, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.WorkPackages, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.Assignments, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.LatestEVM, opt => opt.Ignore()); // Set in service

            // CreateControlAccountDto to ControlAccount - handled manually in service
            CreateMap<CreateControlAccountDto, ControlAccount>()
                .ConstructUsing((src, ctx) => 
                    throw new NotImplementedException("Use factory method in service"));

            // UpdateControlAccountDto to ControlAccount - handled manually in service
            CreateMap<UpdateControlAccountDto, ControlAccount>()
                .ForMember(dest => dest.Name, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.Description, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.BAC, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.ContingencyReserve, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.ManagementReserve, opt => opt.Ignore()) // Updated via method
                .ForMember(dest => dest.CAMUserId, opt => opt.Ignore()); // Updated via method

            // ControlAccountAssignment mappings
            CreateMap<ControlAccountAssignment, ControlAccountAssignmentDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty));

            CreateMap<CreateControlAccountAssignmentDto, ControlAccountAssignment>()
                .ConstructUsing((src, ctx) => 
                    throw new NotImplementedException("Use factory method in service"));

            // EVMRecord to EVMSummaryDto
            CreateMap<EVMRecord, EVMSummaryDto>()
                .ForMember(dest => dest.CV, opt => opt.MapFrom(src => src.EV - src.AC))
                .ForMember(dest => dest.SV, opt => opt.MapFrom(src => src.EV - src.PV))
                .ForMember(dest => dest.CPI, opt => opt.MapFrom(src => src.AC > 0 ? src.EV / src.AC : 0))
                .ForMember(dest => dest.SPI, opt => opt.MapFrom(src => src.PV > 0 ? src.EV / src.PV : 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 
                    src.CPI >= 0.95m && src.SPI >= 0.95m ? EVMStatus.OnTrack :
                    src.CPI < 0.9m || src.SPI < 0.9m ? EVMStatus.AtRisk :
                    EVMStatus.Warning));
        }
    }
}