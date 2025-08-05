using Core.DTOs.Common;
using Core.DTOs.Cost.CommitmentItems;
using Core.DTOs.Cost.Commitments;
using Core.DTOs.Cost.CommitmentWorkPackage;

namespace Application.Interfaces.Cost;

/// <summary>
/// Service interface for Commitment management
/// </summary>
public interface ICommitmentService
{
    // Query operations
    Task<PagedResult<CommitmentListDto>> GetCommitmentsAsync(CommitmentFilterDto filter);
    Task<CommitmentDetailDto?> GetCommitmentDetailAsync(Guid commitmentId);
    Task<CommitmentDto?> GetCommitmentAsync(Guid commitmentId);
    Task<List<CommitmentListDto>> GetProjectCommitmentsAsync(Guid projectId);
    Task<CommitmentSummaryDto> GetProjectCommitmentSummaryAsync(Guid projectId);

    // CRUD operations
    Task<CommitmentDto> CreateCommitmentAsync(CreateCommitmentDto dto);
    Task<CommitmentDto> UpdateCommitmentAsync(Guid commitmentId, UpdateCommitmentDto dto);
    Task<bool> DeleteCommitmentAsync(Guid commitmentId);

    // Workflow operations
    Task<CommitmentDto> SubmitForApprovalAsync(Guid commitmentId);
    Task<CommitmentDto> ApproveCommitmentAsync(Guid commitmentId, ApproveCommitmentDto dto);
    Task<CommitmentDto> RejectCommitmentAsync(Guid commitmentId, CancelCommitmentDto dto);
    Task<CommitmentDto> ActivateCommitmentAsync(Guid commitmentId);
    Task<CommitmentDto> ReviseCommitmentAsync(Guid commitmentId, ReviseCommitmentDto dto);
    Task<CommitmentDto> CloseCommitmentAsync(Guid commitmentId);
    Task<CommitmentDto> CancelCommitmentAsync(Guid commitmentId, CancelCommitmentDto dto);

    // Work Package allocations
    Task<CommitmentDto> AddWorkPackageAllocationAsync(Guid commitmentId, CommitmentWorkPackageAllocationDto dto);
    Task<CommitmentDto> UpdateWorkPackageAllocationAsync(Guid commitmentId, Guid allocationId, decimal newAmount);
    Task<bool> RemoveWorkPackageAllocationAsync(Guid commitmentId, Guid allocationId);
    Task<List<CommitmentWorkPackageDto>> GetWorkPackageAllocationsAsync(Guid commitmentId);

    // Commitment Items management
    Task<CommitmentDto> AddCommitmentItemAsync(Guid commitmentId, CreateCommitmentItemDto dto);
    Task<CommitmentDto> UpdateCommitmentItemAsync(Guid commitmentId, Guid itemId, UpdateCommitmentItemDto dto);
    Task<bool> RemoveCommitmentItemAsync(Guid commitmentId, Guid itemId);
    Task<List<CommitmentItemDto>> GetCommitmentItemsAsync(Guid commitmentId);

    // Financial operations
    Task<CommitmentDto> RecordInvoiceAsync(Guid commitmentId, RecordCommitmentInvoiceDto dto);
    Task<CommitmentDto> UpdatePerformanceAsync(Guid commitmentId, UpdateCommitmentPerformanceDto dto);

    // Reporting
    Task<List<CommitmentRevisionDto>> GetCommitmentRevisionsAsync(Guid commitmentId);
    Task<CommitmentFinancialSummary> GetFinancialSummaryAsync(Guid commitmentId);
    Task<CommitmentPerformanceMetrics> GetPerformanceMetricsAsync(Guid commitmentId);
    Task<byte[]> ExportCommitmentsAsync(CommitmentFilterDto filter, string format = "xlsx");

    // Validation
    Task<bool> IsCommitmentNumberUniqueAsync(string commitmentNumber, Guid? excludeId = null);
    Task<bool> CanDeleteCommitmentAsync(Guid commitmentId);
    Task<bool> CanReviseCommitmentAsync(Guid commitmentId, decimal newAmount);
}
