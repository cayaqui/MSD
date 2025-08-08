using Core.DTOs.Common;
using Core.DTOs.Progress.Milestones;
using Result = Core.Results.Result;
using MilestoneDto = Core.DTOs.Progress.Milestones.MilestoneDto;
using MilestoneFilterDto = Core.DTOs.Progress.Milestones.MilestoneFilterDto;
using UpdateMilestoneProgressDto = Core.DTOs.Progress.Milestones.UpdateMilestoneProgressDto;
using CompleteMilestoneDto = Core.DTOs.Progress.Milestones.CompleteMilestoneDto;
using ApproveMilestoneDto = Core.DTOs.Progress.Milestones.ApproveMilestoneDto;

namespace Application.Interfaces.Progress;

public interface IMilestoneService
{
    // Milestone Management
    Task<PagedResult<MilestoneDto>> GetMilestonesAsync(MilestoneFilterDto filter, CancellationToken cancellationToken = default);
    Task<MilestoneDto?> GetMilestoneByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<MilestoneDto>> GetProjectMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<MilestoneDto>> GetUpcomingMilestonesAsync(Guid projectId, int days = 30, CancellationToken cancellationToken = default);
    Task<Core.Results.Result<Guid>> CreateMilestoneAsync(CreateMilestoneDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> UpdateMilestoneAsync(Guid id, UpdateMilestoneDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeleteMilestoneAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    
    // Progress Management
    Task<Result> UpdateMilestoneProgressAsync(Guid id, UpdateMilestoneProgressDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> CompleteMilestoneAsync(Guid id, CompleteMilestoneDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> ApproveMilestoneAsync(Guid id, ApproveMilestoneDto dto, Guid userId, CancellationToken cancellationToken = default);
    
    // Financial Management
    Task<Result> SetPaymentTermsAsync(Guid milestoneId, decimal amount, string currency, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> TriggerPaymentAsync(Guid milestoneId, Guid userId, CancellationToken cancellationToken = default);
    Task<PaymentSummaryDto> GetPaymentSummaryAsync(Guid projectId, string currency = "USD", CancellationToken cancellationToken = default);
    
    // Dashboard
    Task<MilestoneDashboardDto> GetMilestoneDashboardAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    // Dependencies
    Task<Result> SetMilestoneDependenciesAsync(Guid milestoneId, string[]? predecessors, string[]? successors, Guid userId, CancellationToken cancellationToken = default);
    Task<List<string>> ValidateDependenciesAsync(Guid milestoneId, string[] predecessors, string[] successors, CancellationToken cancellationToken = default);
    
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