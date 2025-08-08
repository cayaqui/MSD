using Core.DTOs.Common;
using Core.DTOs.Cost.Commitments;
using Core.DTOs.Cost.CommitmentItems;
using Core.DTOs.Cost.CommitmentWorkPackage;

namespace Web.Services.Interfaces.Cost;

/// <summary>
/// Interface for commitment API operations
/// </summary>
public interface ICommitmentApiService
{
    // Query Operations
    Task<PagedResult<CommitmentListDto>> GetCommitmentsAsync(CommitmentFilterDto? filter = null, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> GetCommitmentByIdAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    Task<CommitmentDetailDto?> GetCommitmentDetailAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    Task<CommitmentSummaryDto?> GetCommitmentSummaryAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<CommitmentListDto>?> GetProjectCommitmentsAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<CommitmentItemDto>?> GetCommitmentItemsAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    Task<List<CommitmentRevisionDto>?> GetCommitmentRevisionsAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    Task<CommitmentFinancialSummary?> GetFinancialSummaryAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    Task<CommitmentPerformanceMetrics?> GetPerformanceMetricsAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    
    // Command Operations
    Task<CommitmentDto?> CreateCommitmentAsync(CreateCommitmentDto dto, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> UpdateCommitmentAsync(Guid commitmentId, UpdateCommitmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCommitmentAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    
    // Workflow Operations
    Task<CommitmentDto?> SubmitForApprovalAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> ApproveCommitmentAsync(Guid commitmentId, ApproveCommitmentDto dto, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> RejectCommitmentAsync(Guid commitmentId, CancelCommitmentDto dto, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> ActivateCommitmentAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> ReviseCommitmentAsync(Guid commitmentId, ReviseCommitmentDto dto, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> CloseCommitmentAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> CancelCommitmentAsync(Guid commitmentId, CancelCommitmentDto dto, CancellationToken cancellationToken = default);
    
    // Work Package Operations
    Task<CommitmentDto?> AddWorkPackageAllocationAsync(Guid commitmentId, CommitmentWorkPackageAllocationDto dto, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> UpdateWorkPackageAllocationAsync(Guid commitmentId, Guid allocationId, decimal amount, CancellationToken cancellationToken = default);
    Task<bool> RemoveWorkPackageAllocationAsync(Guid commitmentId, Guid allocationId, CancellationToken cancellationToken = default);
    Task<List<CommitmentWorkPackageDto>?> GetWorkPackageAllocationsAsync(Guid commitmentId, CancellationToken cancellationToken = default);
    
    // Item Operations
    Task<CommitmentDto?> AddCommitmentItemAsync(Guid commitmentId, CreateCommitmentItemDto dto, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> UpdateCommitmentItemAsync(Guid commitmentId, Guid itemId, UpdateCommitmentItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> RemoveCommitmentItemAsync(Guid commitmentId, Guid itemId, CancellationToken cancellationToken = default);
    
    // Financial Operations
    Task<CommitmentDto?> RecordInvoiceAsync(Guid commitmentId, RecordCommitmentInvoiceDto dto, CancellationToken cancellationToken = default);
    Task<CommitmentDto?> UpdatePerformanceAsync(Guid commitmentId, UpdateCommitmentPerformanceDto dto, CancellationToken cancellationToken = default);
    
    // Validation Operations
    Task<bool> ValidateCommitmentNumberAsync(string number, Guid? excludeId = null, CancellationToken cancellationToken = default);
    
    // Export Operations
    Task<byte[]?> ExportCommitmentsAsync(CommitmentFilterDto filter, string format = "xlsx", CancellationToken cancellationToken = default);
}