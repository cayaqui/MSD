using Core.DTOs.Common;
using MilestoneDto = Core.DTOs.Progress.Milestones.MilestoneDto;
using Core.DTOs.Progress.Milestones;

namespace Web.Services.Interfaces.Progress;

public interface IMilestoneApiService
{
    // Milestone Management
    Task<PagedResult<MilestoneDto>> GetMilestonesAsync(MilestoneFilterDto filter, CancellationToken cancellationToken = default);
    Task<MilestoneDto?> GetMilestoneByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<MilestoneDto>> GetProjectMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<MilestoneDto>> GetUpcomingMilestonesAsync(Guid projectId, int days = 30, CancellationToken cancellationToken = default);
    Task<List<MilestoneDto>> GetOverdueMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<MilestoneDto>> GetCriticalMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<MilestoneDto>> GetContractualMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Guid> CreateMilestoneAsync(CreateMilestoneDto dto, CancellationToken cancellationToken = default);
    Task UpdateMilestoneAsync(Guid id, UpdateMilestoneDto dto, CancellationToken cancellationToken = default);
    Task DeleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Progress Management
    Task UpdateMilestoneProgressAsync(Guid id, UpdateMilestoneProgressDto dto, CancellationToken cancellationToken = default);
    Task CompleteMilestoneAsync(Guid id, CompleteMilestoneDto dto, CancellationToken cancellationToken = default);
    Task ApproveMilestoneAsync(Guid id, ApproveMilestoneDto dto, CancellationToken cancellationToken = default);
    
    // Financial Management
    Task SetPaymentTermsAsync(Guid milestoneId, decimal amount, string currency, CancellationToken cancellationToken = default);
    Task TriggerPaymentAsync(Guid milestoneId, CancellationToken cancellationToken = default);
    Task<PaymentSummaryDto> GetPaymentSummaryAsync(Guid projectId, string currency = "USD", CancellationToken cancellationToken = default);
    
    // Dependencies
    Task SetMilestoneDependenciesAsync(Guid milestoneId, string[]? predecessors, string[]? successors, CancellationToken cancellationToken = default);
    Task<List<string>> ValidateDependenciesAsync(Guid milestoneId, string[] predecessors, string[] successors, CancellationToken cancellationToken = default);
    
    // Dashboard
    Task<MilestoneDashboardDto> GetMilestoneDashboardAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> ValidateMilestoneCodeAsync(string code, Guid projectId, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> CanDeleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CanCompleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default);
}

public record PaymentSummaryDto
{
    public Guid ProjectId { get; init; }
    public string Currency { get; init; } = "USD";
    public decimal TotalPaymentAmount { get; init; }
    public decimal TriggeredPaymentAmount { get; init; }
    public decimal PendingPaymentAmount { get; init; }
    public decimal PaymentProgress { get; init; }
}