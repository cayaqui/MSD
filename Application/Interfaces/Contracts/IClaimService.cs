using Application.Interfaces.Common;
using Core.DTOs.Contracts.ChangeOrders;
using Core.DTOs.Contracts.Claims;
using Core.Enums.Contracts;
using Domain.Entities.Contracts;

namespace Application.Interfaces.Contracts;

public interface IClaimService : IBaseService<ClaimDto, CreateClaimDto, UpdateClaimDto>
{
    // Claim queries
    Task<IEnumerable<ClaimDto>> GetByContractAsync(Guid contractId);
    Task<ClaimDto?> GetByClaimNumberAsync(string claimNumber);
    Task<IEnumerable<ClaimDto>> SearchAsync(ClaimFilterDto filter);
    Task<IEnumerable<ClaimDto>> GetByDirectionAsync(ClaimDirection direction);
    Task<IEnumerable<ClaimDto>> GetPendingAsync();
    Task<IEnumerable<ClaimDto>> GetOverdueResponseAsync();
    
    // Claim workflow
    Task<ClaimDto?> NotifyAsync(Guid claimId);
    Task<ClaimDto?> SubmitAsync(Guid claimId);
    Task<ClaimDto?> AssessAsync(Guid claimId, AssessClaimDto dto);
    Task<ClaimDto?> ResolveAsync(Guid claimId, ResolveClaimDto dto);
    Task<ClaimDto?> WithdrawAsync(Guid claimId, string reason);
    Task<ClaimDto?> EscalateAsync(Guid claimId, string escalationType);
    
    // Time bar checking
    Task<bool> CheckTimeBarAsync(Guid claimId, int notificationPeriodDays);
    Task<IEnumerable<ClaimDto>> GetTimeBarredAsync();
    Task<IEnumerable<ClaimDto>> GetAtRiskOfTimeBarAsync(int daysWarning = 7);
    
    // Financial tracking
    Task<ClaimDto?> RecordPaymentAsync(Guid claimId, decimal amount);
    Task<decimal> GetTotalClaimedAsync(Guid contractId, ClaimDirection? direction = null);
    Task<decimal> GetTotalApprovedAsync(Guid contractId, ClaimDirection? direction = null);
    Task<decimal> GetTotalPaidAsync(Guid contractId, ClaimDirection? direction = null);
    Task<decimal> GetOutstandingAsync(Guid contractId, ClaimDirection? direction = null);
    
    // Related items
    Task<bool> LinkToChangeOrderAsync(Guid claimId, Guid changeOrderId, string relationType);
    Task<bool> UnlinkFromChangeOrderAsync(Guid claimId, Guid changeOrderId);
    Task<IEnumerable<ChangeOrderDto>> GetRelatedChangeOrdersAsync(Guid claimId);
    
    Task<bool> LinkToClaimAsync(Guid claimId, Guid relatedClaimId, string relationType);
    Task<bool> UnlinkFromClaimAsync(Guid claimId, Guid relatedClaimId);
    Task<IEnumerable<ClaimDto>> GetRelatedClaimsAsync(Guid claimId);
    
    // Assessment and merit
    Task<IEnumerable<ClaimDto>> GetByMeritAsync(bool hasMerit);
    Task<Dictionary<ClaimType, decimal>> GetClaimsByTypeAsync(Guid contractId);
    Task<Dictionary<ClaimResolution, int>> GetResolutionSummaryAsync(Guid? contractId = null);
    
    // Documents
    Task<bool> AttachDocumentAsync(Guid claimId, Guid documentId, string documentType);
    Task<bool> RemoveDocumentAsync(Guid claimId, Guid documentId);
    Task<IEnumerable<ClaimDocumentDto>> GetDocumentsAsync(Guid claimId);
    Task<bool> ValidateDocumentationAsync(Guid claimId);
    
    // Reporting
    Task<IEnumerable<ClaimDto>> GetHighValueClaimsAsync(decimal threshold);
    Task<IEnumerable<ClaimDto>> GetDisputedClaimsAsync();
    Task<Dictionary<ClaimStatus, int>> GetStatusSummaryAsync(Guid? contractId = null);
    Task<Dictionary<string, decimal>> GetMonthlyClaimTrendAsync(Guid contractId, int months = 12);
    
    // Export
    Task<byte[]> ExportClaimRegisterAsync(Guid contractId);
    Task<byte[]> GenerateClaimReportAsync(Guid claimId);
}