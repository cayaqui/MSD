using Application.Interfaces.Common;
using Core.DTOs.Contracts.ChangeOrders;
using Core.DTOs.Contracts.ContractMilestones;
using Core.Enums.Change;
using Core.Enums.Cost;
namespace Application.Interfaces.Contracts;

public interface IChangeOrderService : IBaseService<ChangeOrderDto, CreateChangeOrderDto, UpdateChangeOrderDto>
{
    // Change Order queries
    Task<IEnumerable<ChangeOrderDto>> GetByContractAsync(Guid contractId);
    Task<ChangeOrderDto?> GetByChangeOrderNumberAsync(string changeOrderNumber);
    Task<IEnumerable<ChangeOrderDto>> SearchAsync(ChangeOrderFilterDto filter);
    Task<IEnumerable<ChangeOrderDto>> GetPendingAsync(Guid? contractId = null);
    Task<IEnumerable<ChangeOrderDto>> GetByStatusAsync(ChangeOrderStatus status, Guid? contractId = null);
    
    // Workflow operations
    Task<ChangeOrderDto?> SubmitAsync(Guid changeOrderId, string submittedBy);
    Task<ChangeOrderDto?> ReviewAsync(Guid changeOrderId, string reviewedBy, string comments);
    Task<ChangeOrderDto?> ApproveAsync(Guid changeOrderId, ApproveChangeOrderDto dto, string approvedBy);
    Task<ChangeOrderDto?> RejectAsync(Guid changeOrderId, RejectChangeOrderDto dto, string rejectedBy);
    Task<ChangeOrderDto?> WithdrawAsync(Guid changeOrderId, string reason);
    
    // Implementation tracking
    Task<ChangeOrderDto?> StartImplementationAsync(Guid changeOrderId);
    Task<ChangeOrderDto?> UpdateProgressAsync(Guid changeOrderId, decimal percentageComplete);
    Task<ChangeOrderDto?> CompleteImplementationAsync(Guid changeOrderId, decimal actualCost);
    
    // Related items
    Task<bool> LinkToMilestoneAsync(Guid changeOrderId, Guid milestoneId, string impactType, string impactDescription);
    Task<bool> UnlinkFromMilestoneAsync(Guid changeOrderId, Guid milestoneId);
    Task<IEnumerable<ContractMilestoneDto>> GetAffectedMilestonesAsync(Guid changeOrderId);
    
    Task<bool> LinkToChangeOrderAsync(Guid changeOrderId, Guid relatedChangeOrderId, string relationType);
    Task<bool> UnlinkFromChangeOrderAsync(Guid changeOrderId, Guid relatedChangeOrderId);
    Task<IEnumerable<ChangeOrderDto>> GetRelatedChangeOrdersAsync(Guid changeOrderId);
    
    // Financial impact
    Task<decimal> CalculateTotalImpactAsync(Guid contractId);
    Task<int> CalculateTotalScheduleImpactAsync(Guid contractId);
    
    // Documents
    Task<bool> AttachDocumentAsync(Guid changeOrderId, Guid documentId, string documentType);
    Task<bool> RemoveDocumentAsync(Guid changeOrderId, Guid documentId);
    Task<IEnumerable<ChangeOrderDocumentDto>> GetDocumentsAsync(Guid changeOrderId);
    
    // Reporting
    Task<IEnumerable<ChangeOrderDto>> GetOverdueAsync();
    Task<IEnumerable<ChangeOrderDto>> GetHighPriorityAsync();
    Task<Dictionary<ChangeOrderStatus, int>> GetStatusSummaryAsync(Guid contractId);
    Task<Dictionary<ChangeOrderType, decimal>> GetCostByTypeAsync(Guid contractId);
}