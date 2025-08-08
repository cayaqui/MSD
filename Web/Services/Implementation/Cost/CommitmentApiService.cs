using Core.DTOs.Common;
using Core.DTOs.Cost.Commitments;
using Core.DTOs.Cost.CommitmentItems;
using Core.DTOs.Cost.CommitmentWorkPackage;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Cost;

namespace Web.Services.Implementation.Cost;

/// <summary>
/// Service implementation for commitment API operations
/// </summary>
public class CommitmentApiService : ICommitmentApiService
{
    private readonly IApiService _apiService;
    private readonly ILoggingService _logger;
    private const string BaseEndpoint = "api/commitments";

    public CommitmentApiService(IApiService apiService, ILoggingService logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // Query Operations
    public async Task<PagedResult<CommitmentListDto>> GetCommitmentsAsync(CommitmentFilterDto? filter = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Searching commitments with filter");
            filter ??= new CommitmentFilterDto();
            return await _apiService.PostAsync<CommitmentFilterDto, PagedResult<CommitmentListDto>>($"{BaseEndpoint}/search", filter) 
                ?? new PagedResult<CommitmentListDto>(new List<CommitmentListDto>(), filter.PageNumber, filter.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching commitments");
            return new PagedResult<CommitmentListDto>(new List<CommitmentListDto>(), filter?.PageNumber ?? 1, filter?.PageSize ?? 10);
        }
    }

    public async Task<CommitmentDto?> GetCommitmentByIdAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting commitment by ID: {commitmentId}");
            return await _apiService.GetAsync<CommitmentDto>($"{BaseEndpoint}/{commitmentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting commitment {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentDetailDto?> GetCommitmentDetailAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting commitment detail for ID: {commitmentId}");
            return await _apiService.GetAsync<CommitmentDetailDto>($"{BaseEndpoint}/{commitmentId}/detail");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting commitment detail {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentSummaryDto?> GetCommitmentSummaryAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting commitment summary for project: {projectId}");
            return await _apiService.GetAsync<CommitmentSummaryDto>($"{BaseEndpoint}/project/{projectId}/summary");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting commitment summary for project {projectId}");
            return null;
        }
    }

    public async Task<List<CommitmentListDto>?> GetProjectCommitmentsAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting commitments for project: {projectId}");
            return await _apiService.GetAsync<List<CommitmentListDto>>($"{BaseEndpoint}/project/{projectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting project commitments for {projectId}");
            return null;
        }
    }

    public async Task<List<CommitmentItemDto>?> GetCommitmentItemsAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting commitment items for ID: {commitmentId}");
            return await _apiService.GetAsync<List<CommitmentItemDto>>($"{BaseEndpoint}/{commitmentId}/items");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting commitment items for {commitmentId}");
            return null;
        }
    }

    public async Task<List<CommitmentRevisionDto>?> GetCommitmentRevisionsAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting commitment revisions for ID: {commitmentId}");
            return await _apiService.GetAsync<List<CommitmentRevisionDto>>($"{BaseEndpoint}/{commitmentId}/revisions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting commitment revisions for {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentFinancialSummary?> GetFinancialSummaryAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting financial summary for commitment: {commitmentId}");
            return await _apiService.GetAsync<CommitmentFinancialSummary>($"{BaseEndpoint}/{commitmentId}/financial-summary");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting financial summary for {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentPerformanceMetrics?> GetPerformanceMetricsAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting performance metrics for commitment: {commitmentId}");
            return await _apiService.GetAsync<CommitmentPerformanceMetrics>($"{BaseEndpoint}/{commitmentId}/performance-metrics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting performance metrics for {commitmentId}");
            return null;
        }
    }

    // Command Operations
    public async Task<CommitmentDto?> CreateCommitmentAsync(CreateCommitmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Creating new commitment: {dto.CommitmentNumber}");
            return await _apiService.PostAsync<CreateCommitmentDto, CommitmentDto>(BaseEndpoint, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating commitment: {dto.CommitmentNumber}");
            return null;
        }
    }

    public async Task<CommitmentDto?> UpdateCommitmentAsync(Guid commitmentId, UpdateCommitmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating commitment: {commitmentId}");
            return await _apiService.PutAsync<UpdateCommitmentDto, CommitmentDto>($"{BaseEndpoint}/{commitmentId}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating commitment: {commitmentId}");
            return null;
        }
    }

    public async Task<bool> DeleteCommitmentAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Deleting commitment: {commitmentId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{commitmentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting commitment: {commitmentId}");
            return false;
        }
    }

    // Workflow Operations
    public async Task<CommitmentDto?> SubmitForApprovalAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Submitting commitment for approval: {commitmentId}");
            return await _apiService.PostAsync<object, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/submit", new { });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error submitting commitment for approval: {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentDto?> ApproveCommitmentAsync(Guid commitmentId, ApproveCommitmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Approving commitment: {commitmentId}");
            return await _apiService.PostAsync<ApproveCommitmentDto, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/approve", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving commitment: {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentDto?> RejectCommitmentAsync(Guid commitmentId, CancelCommitmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Rejecting commitment: {commitmentId}");
            return await _apiService.PostAsync<CancelCommitmentDto, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/reject", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error rejecting commitment: {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentDto?> ActivateCommitmentAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Activating commitment: {commitmentId}");
            return await _apiService.PostAsync<object, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/activate", new { });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error activating commitment: {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentDto?> ReviseCommitmentAsync(Guid commitmentId, ReviseCommitmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Revising commitment: {commitmentId}");
            return await _apiService.PostAsync<ReviseCommitmentDto, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/revise", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error revising commitment: {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentDto?> CloseCommitmentAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Closing commitment: {commitmentId}");
            return await _apiService.PostAsync<object, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/close", new { });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error closing commitment: {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentDto?> CancelCommitmentAsync(Guid commitmentId, CancelCommitmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Cancelling commitment: {commitmentId}");
            return await _apiService.PostAsync<CancelCommitmentDto, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/cancel", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error cancelling commitment: {commitmentId}");
            return null;
        }
    }

    // Work Package Operations
    public async Task<CommitmentDto?> AddWorkPackageAllocationAsync(Guid commitmentId, CommitmentWorkPackageAllocationDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Adding work package allocation to commitment: {commitmentId}");
            return await _apiService.PostAsync<CommitmentWorkPackageAllocationDto, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/allocations", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding work package allocation to commitment: {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentDto?> UpdateWorkPackageAllocationAsync(Guid commitmentId, Guid allocationId, decimal amount, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating work package allocation: {allocationId} for commitment: {commitmentId}");
            return await _apiService.PutAsync<object, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/allocations/{allocationId}", new { Amount = amount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating work package allocation: {allocationId}");
            return null;
        }
    }

    public async Task<bool> RemoveWorkPackageAllocationAsync(Guid commitmentId, Guid allocationId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Removing work package allocation: {allocationId} from commitment: {commitmentId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{commitmentId}/allocations/{allocationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing work package allocation: {allocationId}");
            return false;
        }
    }

    public async Task<List<CommitmentWorkPackageDto>?> GetWorkPackageAllocationsAsync(Guid commitmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Getting work package allocations for commitment: {commitmentId}");
            return await _apiService.GetAsync<List<CommitmentWorkPackageDto>>($"{BaseEndpoint}/{commitmentId}/allocations");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting work package allocations for commitment: {commitmentId}");
            return null;
        }
    }

    // Item Operations
    public async Task<CommitmentDto?> AddCommitmentItemAsync(Guid commitmentId, CreateCommitmentItemDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Adding item to commitment: {commitmentId}");
            return await _apiService.PostAsync<CreateCommitmentItemDto, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/items", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding item to commitment: {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentDto?> UpdateCommitmentItemAsync(Guid commitmentId, Guid itemId, UpdateCommitmentItemDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating commitment item: {itemId} for commitment: {commitmentId}");
            return await _apiService.PutAsync<UpdateCommitmentItemDto, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/items/{itemId}", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating commitment item: {itemId}");
            return null;
        }
    }

    public async Task<bool> RemoveCommitmentItemAsync(Guid commitmentId, Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Removing item: {itemId} from commitment: {commitmentId}");
            return await _apiService.DeleteAsync($"{BaseEndpoint}/{commitmentId}/items/{itemId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing commitment item: {itemId}");
            return false;
        }
    }

    // Financial Operations
    public async Task<CommitmentDto?> RecordInvoiceAsync(Guid commitmentId, RecordCommitmentInvoiceDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Recording invoice for commitment: {commitmentId}");
            return await _apiService.PostAsync<RecordCommitmentInvoiceDto, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/invoice", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error recording invoice for commitment: {commitmentId}");
            return null;
        }
    }

    public async Task<CommitmentDto?> UpdatePerformanceAsync(Guid commitmentId, UpdateCommitmentPerformanceDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Updating performance for commitment: {commitmentId}");
            return await _apiService.PostAsync<UpdateCommitmentPerformanceDto, CommitmentDto>($"{BaseEndpoint}/{commitmentId}/performance", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating performance for commitment: {commitmentId}");
            return null;
        }
    }

    // Validation Operations
    public async Task<bool> ValidateCommitmentNumberAsync(string number, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug($"Validating commitment number: {number}");
            var query = excludeId.HasValue ? $"?excludeId={excludeId.Value}" : "";
            return await _apiService.GetAsync<bool>($"{BaseEndpoint}/validate/number/{Uri.EscapeDataString(number)}{query}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error validating commitment number: {number}");
            return false;
        }
    }

    // Export Operations
    public async Task<byte[]?> ExportCommitmentsAsync(CommitmentFilterDto filter, string format = "xlsx", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInfo($"Exporting commitments to {format} format");
            var response = await _apiService.PostAsync<object, byte[]>($"{BaseEndpoint}/export", new { Filter = filter, Format = format });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error exporting commitments");
            return null;
        }
    }
}