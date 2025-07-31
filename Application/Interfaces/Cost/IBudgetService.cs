using Core.DTOs.Budget;

namespace Application.Interfaces.Cost;

/// <summary>
/// Interface for Budget management service
/// </summary>
public interface IBudgetService
{
    // Query Operations
    Task<PagedResult<BudgetDto>> GetBudgetsAsync(
        Guid projectId,
        QueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<BudgetDetailDto?> GetBudgetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<BudgetDetailDto?> GetCurrentBaselineBudgetAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<List<BudgetItemDto>> GetBudgetItemsAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default);

    // Command Operations
    Task<Result<Guid>> CreateBudgetAsync(
        CreateBudgetDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateBudgetAsync(
        Guid id,
        UpdateBudgetDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> SubmitBudgetForApprovalAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> ApproveBudgetAsync(
        Guid id,
        ApproveBudgetDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> RejectBudgetAsync(
        Guid id,
        RejectBudgetDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> SetBudgetAsBaselineAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> LockBudgetAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteBudgetAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    // Budget Item Operations
    Task<Result<Guid>> AddBudgetItemAsync(
        CreateBudgetItemDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateBudgetItemAsync(
        Guid itemId,
        decimal quantity,
        decimal unitRate,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveBudgetItemAsync(
        Guid itemId,
        string userId,
        CancellationToken cancellationToken = default);

    // Revision Operations
    Task<Result<Guid>> CreateBudgetRevisionAsync(
        Guid budgetId,
        CreateBudgetRevisionDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> ApproveBudgetRevisionAsync(
        Guid revisionId,
        string userId,
        CancellationToken cancellationToken = default);
}
