using Core.DTOs.Common;
using Core.DTOs.Progress.Milestones;
using Web.Services.Interfaces;
using Web.Services.Interfaces.Progress;

namespace Web.Services.Implementation.Progress;

public class MilestoneApiService : IMilestoneApiService
{
    private readonly IApiService _apiService;
    private const string BaseUrl = "api/milestones";

    public MilestoneApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<PagedResult<MilestoneDto>> GetMilestonesAsync(MilestoneFilterDto filter, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["projectId"] = filter.ProjectId?.ToString(),
            ["wbsElementId"] = filter.WBSElementId?.ToString(),
            ["status"] = filter.Status?.ToString(),
            ["milestoneType"] = filter.MilestoneType?.ToString(),
            ["isCritical"] = filter.IsCritical?.ToString(),
            ["isContractual"] = filter.IsContractual?.ToString(),
            ["isPaymentTriggered"] = filter.IsPaymentTriggered?.ToString(),
            ["searchTerm"] = filter.SearchTerm,
            ["sortBy"] = filter.SortBy,
            ["sortDescending"] = filter.SortDescending.ToString(),
            ["pageNumber"] = filter.PageNumber.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };

        return await _apiService.GetAsync<PagedResult<MilestoneDto>>(BaseUrl, queryParams, cancellationToken);
    }

    public async Task<MilestoneDto?> GetMilestoneByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<MilestoneDto>($"{BaseUrl}/{id}", cancellationToken);
    }

    public async Task<List<MilestoneDto>> GetProjectMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<List<MilestoneDto>>($"{BaseUrl}/project/{projectId}", cancellationToken);
    }

    public async Task<List<MilestoneDto>> GetUpcomingMilestonesAsync(Guid projectId, int days = 30, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?> { ["days"] = days.ToString() };
        return await _apiService.GetAsync<List<MilestoneDto>>($"{BaseUrl}/project/{projectId}/upcoming", queryParams, cancellationToken);
    }

    public async Task<List<MilestoneDto>> GetOverdueMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<List<MilestoneDto>>($"{BaseUrl}/project/{projectId}/overdue", cancellationToken);
    }

    public async Task<List<MilestoneDto>> GetCriticalMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<List<MilestoneDto>>($"{BaseUrl}/project/{projectId}/critical", cancellationToken);
    }

    public async Task<List<MilestoneDto>> GetContractualMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<List<MilestoneDto>>($"{BaseUrl}/project/{projectId}/contractual", cancellationToken);
    }

    public async Task<Guid> CreateMilestoneAsync(CreateMilestoneDto dto, CancellationToken cancellationToken = default)
    {
        return await _apiService.PostAsync<CreateMilestoneDto, Guid>(BaseUrl, dto, cancellationToken);
    }

    public async Task UpdateMilestoneAsync(Guid id, UpdateMilestoneDto dto, CancellationToken cancellationToken = default)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}", dto, cancellationToken);
    }

    public async Task DeleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _apiService.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
    }

    public async Task UpdateMilestoneProgressAsync(Guid id, UpdateMilestoneProgressDto dto, CancellationToken cancellationToken = default)
    {
        await _apiService.PutAsync($"{BaseUrl}/{id}/progress", dto, cancellationToken);
    }

    public async Task CompleteMilestoneAsync(Guid id, CompleteMilestoneDto dto, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{id}/complete", dto, cancellationToken);
    }

    public async Task ApproveMilestoneAsync(Guid id, ApproveMilestoneDto dto, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{id}/approve", dto, cancellationToken);
    }

    public async Task SetPaymentTermsAsync(Guid milestoneId, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        await _apiService.PutAsync($"{BaseUrl}/{milestoneId}/payment-terms", new { amount, currency }, cancellationToken);
    }

    public async Task TriggerPaymentAsync(Guid milestoneId, CancellationToken cancellationToken = default)
    {
        await _apiService.PostAsync($"{BaseUrl}/{milestoneId}/trigger-payment", new { }, cancellationToken);
    }

    public async Task<PaymentSummaryDto> GetPaymentSummaryAsync(Guid projectId, string currency = "USD", CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?> { ["currency"] = currency };
        return await _apiService.GetAsync<PaymentSummaryDto>($"{BaseUrl}/project/{projectId}/payment-summary", queryParams, cancellationToken);
    }

    public async Task SetMilestoneDependenciesAsync(Guid milestoneId, string[]? predecessors, string[]? successors, CancellationToken cancellationToken = default)
    {
        await _apiService.PutAsync($"{BaseUrl}/{milestoneId}/dependencies", new { predecessorMilestones = predecessors, successorMilestones = successors }, cancellationToken);
    }

    public async Task<List<string>> ValidateDependenciesAsync(Guid milestoneId, string[] predecessors, string[] successors, CancellationToken cancellationToken = default)
    {
        return await _apiService.PostAsync<object, List<string>>($"{BaseUrl}/validate-dependencies", 
            new { milestoneId, predecessorMilestones = predecessors, successorMilestones = successors }, cancellationToken);
    }

    public async Task<MilestoneDashboardDto> GetMilestoneDashboardAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<MilestoneDashboardDto>($"{BaseUrl}/project/{projectId}/dashboard", cancellationToken);
    }

    public async Task<bool> ValidateMilestoneCodeAsync(string code, Guid projectId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["code"] = code,
            ["projectId"] = projectId.ToString(),
            ["excludeId"] = excludeId?.ToString()
        };
        return await _apiService.GetAsync<bool>($"{BaseUrl}/validate-code", queryParams, cancellationToken);
    }

    public async Task<bool> CanDeleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<bool>($"{BaseUrl}/{id}/can-delete", cancellationToken);
    }

    public async Task<bool> CanCompleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _apiService.GetAsync<bool>($"{BaseUrl}/{id}/can-complete", cancellationToken);
    }
}