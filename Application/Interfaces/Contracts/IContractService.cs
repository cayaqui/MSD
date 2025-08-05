using Application.Interfaces.Common;
using Core.DTOs.Contracts.ChangeOrders;
using Core.DTOs.Contracts.Claims;
using Core.DTOs.Contracts.ContractMilestones;
using Core.DTOs.Contracts.Contracts;
using Core.DTOs.Contracts.Valuations;
using Core.Enums.Contracts;
using Domain.Entities.Contracts.Core;

namespace Application.Interfaces.Contracts;

public interface IContractService : IBaseService<ContractDto, CreateContractDto, UpdateContractDto>
{
    // Contract-specific queries
    Task<IEnumerable<ContractDto>> GetByProjectAsync(Guid projectId);
    Task<IEnumerable<ContractDto>> GetByContractorAsync(Guid contractorId);
    Task<ContractDto?> GetByContractNumberAsync(string contractNumber);
    Task<IEnumerable<ContractDto>> SearchAsync(ContractFilterDto filter);
    Task<ContractSummaryDto> GetSummaryAsync(Guid? projectId = null, Guid? contractorId = null);
    
    // Contract operations
    Task<ContractDto?> ApproveAsync(Guid contractId, string approvedBy, string comments);
    Task<ContractDto?> TerminateAsync(Guid contractId, string reason);
    Task<ContractDto?> CloseAsync(Guid contractId);
    Task<ContractDto?> UpdateStatusAsync(Guid contractId, ContractStatus status);
    
    // Financial operations
    Task<ContractDto?> UpdateFinancialsAsync(Guid contractId);
    Task<decimal> CalculateRetentionAsync(Guid contractId);
    Task<decimal> GetOutstandingAmountAsync(Guid contractId);
    
    // Change Order operations
    Task<ContractDto?> ApplyChangeOrderAsync(Guid contractId, Guid changeOrderId);
    Task<IEnumerable<ChangeOrderDto>> GetChangeOrdersAsync(Guid contractId);
    
    // Milestone operations  
    Task<IEnumerable<ContractMilestoneDto>> GetMilestonesAsync(Guid contractId);
    Task<ContractMilestoneDto?> GetNextMilestoneAsync(Guid contractId);
    Task<IEnumerable<ContractMilestoneDto>> GetOverdueMilestonesAsync(Guid contractId);
    
    // Valuation operations
    Task<IEnumerable<ValuationDto>> GetValuationsAsync(Guid contractId);
    Task<ValuationDto?> GetLatestValuationAsync(Guid contractId);
    Task<ValuationSummaryDto> GetValuationSummaryAsync(Guid contractId);
    
    // Claim operations
    Task<IEnumerable<ClaimDto>> GetClaimsAsync(Guid contractId);
    Task<int> GetOpenClaimsCountAsync(Guid contractId);
    Task<decimal> GetTotalClaimsValueAsync(Guid contractId);
    
    // Risk assessment
    Task<ContractDto?> UpdateRiskLevelAsync(Guid contractId, ContractRiskLevel riskLevel);
    Task<IEnumerable<ContractDto>> GetHighRiskContractsAsync();
    
    // Document operations
    Task<bool> AttachDocumentAsync(Guid contractId, Guid documentId, string documentType);
    Task<bool> RemoveDocumentAsync(Guid contractId, Guid documentId);
    Task<IEnumerable<ContractDocument>> GetDocumentsAsync(Guid contractId);
    
    // Reporting
    Task<IEnumerable<ContractDto>> GetExpiringContractsAsync(int daysAhead = 30);
    Task<IEnumerable<ContractDto>> GetContractsWithExpiredBondsAsync();
    Task<IEnumerable<ContractDto>> GetDelayedContractsAsync();
}