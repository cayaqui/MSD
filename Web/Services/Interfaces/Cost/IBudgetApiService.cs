using Core.DTOs.Common;
using Core.DTOs.Cost.Budgets;
using Core.DTOs.Cost.BudgetItems;
using Core.DTOs.Cost.BudgetRevisions;

namespace Web.Services.Interfaces.Cost;

/// <summary>
/// Interface for budget API operations
/// </summary>
public interface IBudgetApiService
{
    // Query Operations
    Task<PagedResult<BudgetDto>> GetBudgetsAsync(BudgetFilterDto? filter = null, CancellationToken cancellationToken = default);
    Task<BudgetDto?> GetBudgetByIdAsync(Guid budgetId, CancellationToken cancellationToken = default);
    Task<BudgetDetailDto?> GetCurrentBaselineBudgetAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<BudgetItemDto>?> GetBudgetItemsAsync(Guid budgetId, CancellationToken cancellationToken = default);
    
    // Command Operations
    Task<BudgetDto?> CreateBudgetAsync(CreateBudgetDto dto, CancellationToken cancellationToken = default);
    Task<BudgetDto?> UpdateBudgetAsync(Guid budgetId, UpdateBudgetDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteBudgetAsync(Guid budgetId, CancellationToken cancellationToken = default);
    
    // Workflow Operations
    Task<bool> SubmitBudgetForApprovalAsync(Guid budgetId, CancellationToken cancellationToken = default);
    Task<bool> ApproveBudgetAsync(Guid budgetId, ApproveBudgetDto dto, CancellationToken cancellationToken = default);
    Task<bool> RejectBudgetAsync(Guid budgetId, RejectBudgetDto dto, CancellationToken cancellationToken = default);
    Task<bool> SetBudgetAsBaselineAsync(Guid budgetId, CancellationToken cancellationToken = default);
    Task<bool> LockBudgetAsync(Guid budgetId, CancellationToken cancellationToken = default);
    
    // Budget Item Operations
    Task<Guid?> AddBudgetItemAsync(CreateBudgetItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateBudgetItemAsync(Guid itemId, decimal quantity, decimal unitRate, CancellationToken cancellationToken = default);
    Task<bool> RemoveBudgetItemAsync(Guid itemId, CancellationToken cancellationToken = default);
    
    // Revision Operations
    Task<Guid?> CreateBudgetRevisionAsync(Guid budgetId, CreateBudgetRevisionDto dto, CancellationToken cancellationToken = default);
    Task<bool> ApproveBudgetRevisionAsync(Guid revisionId, CancellationToken cancellationToken = default);
}