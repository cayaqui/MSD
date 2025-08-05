using AutoMapper;
using Core.DTOs.Cost.Commitments;
using Core.DTOs.Cost.CommitmentItems;
using Core.DTOs.Cost.CommitmentWorkPackage;
using Domain.Entities.Cost.Commitments;

namespace Application.Mappings.Cost
{
    /// <summary>
    /// AutoMapper profile for Commitment entity mappings
    /// </summary>
    public class CommitmentMappingProfile : Profile
    {
        public CommitmentMappingProfile()
        {
            // Commitment to CommitmentDto
            CreateMap<Commitment, CommitmentDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.ProjectCode, opt => opt.MapFrom(src => src.Project != null ? src.Project.Code : string.Empty))
                .ForMember(dest => dest.BudgetItemCode, opt => opt.MapFrom(src => src.BudgetItem != null ? src.BudgetItem.ItemCode : null))
                .ForMember(dest => dest.BudgetItemName, opt => opt.MapFrom(src => src.BudgetItem != null ? src.BudgetItem.Description : null))
                .ForMember(dest => dest.ContractorCode, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Code : null))
                .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Name : null))
                .ForMember(dest => dest.ControlAccountCode, opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Code : null))
                .ForMember(dest => dest.ControlAccountName, opt => opt.MapFrom(src => src.ControlAccount != null ? src.ControlAccount.Name : null))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted));

            // Commitment to CommitmentListDto
            CreateMap<Commitment, CommitmentListDto>()
                .ForMember(dest => dest.ContractorCode, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Code : null))
                .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Name : null))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted));

            // Commitment to CommitmentDetailDto
            CreateMap<Commitment, CommitmentDetailDto>()
                .IncludeBase<Commitment, CommitmentDto>()
                .ForMember(dest => dest.WorkPackageAllocations, opt => opt.MapFrom(src => src.WorkPackageAllocations))
                .ForMember(dest => dest.Revisions, opt => opt.MapFrom(src => src.Revisions))
                .ForMember(dest => dest.Invoices, opt => opt.Ignore()) // Mapped separately
                .ForMember(dest => dest.FinancialSummary, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.PerformanceMetrics, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.AuditTrail, opt => opt.Ignore()); // Set in service

            // CreateCommitmentDto to Commitment - handled manually in service
            CreateMap<CreateCommitmentDto, Commitment>()
                .ConstructUsing((src, ctx) => 
                    throw new NotImplementedException("Use factory method in service"));

            // UpdateCommitmentDto to Commitment - handled manually in service
            CreateMap<UpdateCommitmentDto, Commitment>()
                .ForAllMembers(opts => opts.Ignore());

            // CommitmentItem mappings
            CreateMap<CommitmentItem, CommitmentItemDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
                .ForMember(dest => dest.NetAmount, opt => opt.MapFrom(src => src.NetAmount))
                .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount))
                .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.LineTotal));

            CreateMap<CreateCommitmentItemDto, CommitmentItem>()
                .ConstructUsing((src, ctx) => 
                    throw new NotImplementedException("Use factory method in service"));

            // CommitmentWorkPackage mappings
            CreateMap<CommitmentWorkPackage, CommitmentWorkPackageDto>()
                .ForMember(dest => dest.WBSCode, opt => opt.MapFrom(src => src.WBSElement != null ? src.WBSElement.Code : string.Empty))
                .ForMember(dest => dest.WBSName, opt => opt.MapFrom(src => src.WBSElement != null ? src.WBSElement.Name : string.Empty))
                .ForMember(dest => dest.WBSDescription, opt => opt.MapFrom(src => src.WBSElement != null ? src.WBSElement.Description : null));

            // CommitmentRevision mappings
            CreateMap<CommitmentRevision, CommitmentRevisionDto>()
                .ForMember(dest => dest.RevisedAmount, opt => opt.MapFrom(src => src.RevisedAmount))
                .ForMember(dest => dest.ChangeAmount, opt => opt.MapFrom(src => src.ChangeAmount))
                .ForMember(dest => dest.ChangePercentage, opt => opt.MapFrom(src => src.ChangePercentage))
                .ForMember(dest => dest.ApprovedBy, opt => opt.MapFrom(src => src.ApprovedBy))
                .ForMember(dest => dest.ApprovalDate, opt => opt.MapFrom(src => src.ApprovalDate));
        }
    }
}