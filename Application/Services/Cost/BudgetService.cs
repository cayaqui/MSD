using Application.Interfaces.Auth;
using Application.Interfaces.Cost;
using Application.Services.Base;
using Core.DTOs.Cost.BudgetItems;
using Core.DTOs.Cost.BudgetRevisions;
using Core.DTOs.Cost.Budgets;
using Core.Enums.Cost;
using Domain.Entities.Cost.Budget;

namespace Application.Services.Cost;

/// <summary>
/// Service implementation for Budget management following Clean Architecture principles
/// </summary>
public class BudgetService
    : BaseService<Budget, BudgetDto, CreateBudgetDto, UpdateBudgetDto>,
        IBudgetService
{
    private readonly ICurrentUserService _currentUserService;

    public BudgetService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ILogger<BudgetService> logger
    )
        : base(unitOfWork, mapper, logger)
    {
        _currentUserService = currentUserService;
    }

    #region Query Operations

    public async Task<PagedResult<BudgetDto>> GetBudgetsAsync(
        Guid projectId,
        QueryParameters parameters,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var query = _unitOfWork
                .Repository<Budget>()
                .QueryNoTracking()
                .Where(b => b.ProjectId == projectId && !b.IsDeleted);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                query = query.Where(b =>
                    b.Name.ToLower().Contains(searchTerm)
                    || b.Version.ToLower().Contains(searchTerm)
                    || (b.Description != null && b.Description.ToLower().Contains(searchTerm))
                );
            }

            // Include related data
            query = query.Include(b => b.Project);

            // Apply sorting
            query = ApplySorting(query, parameters.SortBy, parameters.SortDirection == "desc");

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<BudgetDto>>(items);

            _logger.LogInformation(
                "Retrieved {Count} budgets for project {ProjectId}",
                dtos.Count,
                projectId
            );

            return new PagedResult<BudgetDto>(dtos, parameters.PageNumber, parameters.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budgets for project {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<BudgetDetailDto?> GetCurrentBaselineBudgetAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var budget = await _unitOfWork
                .Repository<Budget>()
                .GetAsync(
                    filter: b => b.ProjectId == projectId && b.IsBaseline && !b.IsDeleted,
                    includeProperties: "Project,BudgetItems.ControlAccount,BudgetItems.WBSElement,Revisions",
                    cancellationToken: cancellationToken
                );

            if (budget == null)
            {
                _logger.LogWarning("No baseline budget found for project {ProjectId}", projectId);
                return null;
            }

            var dto = _mapper.Map<BudgetDetailDto>(budget);

            // Calculate additional metrics
            dto.AllocatedAmount = budget.BudgetItems.Where(i => !i.IsDeleted).Sum(i => i.Amount);

            dto.UnallocatedAmount = budget.TotalAmount - dto.AllocatedAmount;
            dto.AllocationPercentage =
                budget.TotalAmount > 0 ? (dto.AllocatedAmount / budget.TotalAmount) * 100 : 0;

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving baseline budget for project {ProjectId}",
                projectId
            );
            throw;
        }
    }

    public async Task<List<BudgetItemDto>> GetBudgetItemsAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var items = await _unitOfWork
                .Repository<BudgetItem>()
                .GetAllAsync(
                    filter: i => i.BudgetId == budgetId && !i.IsDeleted,
                    includeProperties: "ControlAccount,WBSElement",
                    orderBy: q => q.OrderBy(i => i.ItemCode)
                );

            return _mapper.Map<List<BudgetItemDto>>(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budget items for budget {BudgetId}", budgetId);
            throw;
        }
    }

    #endregion

    #region Create/Update Operations

    public async Task<Result<Guid>> CreateAsync(
        CreateBudgetDto dto,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Validate project exists
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(dto.ProjectId);

            if (project == null)
            {
                return Result<Guid>.Failure("Project not found");
            }

            // Check if project already has a budget with same version
            var existingBudget = await _unitOfWork
                .Repository<Budget>()
                .GetAsync(
                    b => b.ProjectId == dto.ProjectId && b.Version == dto.Version && !b.IsDeleted,
                    cancellationToken: cancellationToken
                );

            if (existingBudget != null)
            {
                return Result<Guid>.Failure(
                    $"Budget with version '{dto.Version}' already exists for this project"
                );
            }

            // Create budget entity
            var budget = new Budget(
                dto.ProjectId,
                dto.Version,
                dto.Name,
                dto.Type,
                dto.TotalAmount,
                dto.Currency
            );

            // Set optional properties
            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                budget.UpdateDescription(dto.Description);
            }

            if (dto.ContingencyPercentage.HasValue)
            {
                budget.SetContingencyPercentage(dto.ContingencyPercentage.Value);
            }

            if (dto.ManagementReservePercentage.HasValue)
            {
                budget.SetManagementReservePercentage(dto.ManagementReservePercentage.Value);
            }

            // Add budget
            await _unitOfWork.Repository<Budget>().AddAsync(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Created budget {BudgetId} for project {ProjectId}",
                budget.Id,
                budget.ProjectId
            );

            return Result<Guid>.Success(budget.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error creating budget for project {ProjectId}", dto.ProjectId);
            return Result<Guid>.Failure($"Failed to create budget: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(
        Guid id,
        UpdateBudgetDto dto,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var budget = await _unitOfWork
                .Repository<Budget>()
                .GetAsync(b => b.Id == id && !b.IsDeleted, cancellationToken: cancellationToken);

            if (budget == null)
            {
                return Result.Failure("Budget not found");
            }

            // Validate business rules
            if (budget.Status != BudgetStatus.Draft)
            {
                return Result.Failure("Only draft budgets can be updated");
            }

            if (budget.IsLocked)
            {
                return Result.Failure("Budget is locked and cannot be updated");
            }

            // Update properties
            budget.UpdateName(dto.Name);

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                budget.UpdateDescription(dto.Description);
            }

            if (dto.TotalAmount.HasValue && dto.TotalAmount.Value != budget.TotalAmount)
            {
                budget.UpdateTotalAmount(dto.TotalAmount.Value);
            }

            if (dto.ContingencyPercentage.HasValue)
            {
                budget.SetContingencyPercentage(dto.ContingencyPercentage.Value);
            }

            if (dto.ManagementReservePercentage.HasValue)
            {
                budget.SetManagementReservePercentage(dto.ManagementReservePercentage.Value);
            }

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Updated budget {BudgetId}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error updating budget {BudgetId}", id);
            return Result.Failure($"Failed to update budget: {ex.Message}");
        }
    }

    #endregion

    #region Workflow Operations

    public async Task<Result> SubmitBudgetForApprovalAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var budget = await _unitOfWork
                .Repository<Budget>()
                .GetAsync(
                    filter: b => b.Id == id && !b.IsDeleted,
                    includeProperties: "BudgetItems",
                    cancellationToken: cancellationToken
                );

            if (budget == null)
            {
                return Result.Failure("Budget not found");
            }

            // Validate business rules
            var validationResult = await ValidateBudgetForApproval(budget);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // Submit for approval
            budget.SubmitForApproval(userId);

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Budget {BudgetId} submitted for approval by {UserId}",
                id,
                userId
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error submitting budget {BudgetId} for approval", id);
            return Result.Failure($"Failed to submit budget for approval: {ex.Message}");
        }
    }

    public async Task<Result> ApproveBudgetAsync(
        Guid id,
        ApproveBudgetDto dto,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var budget = await _unitOfWork
                .Repository<Budget>()
                .GetAsync(b => b.Id == id && !b.IsDeleted, cancellationToken: cancellationToken);

            if (budget == null)
            {
                return Result.Failure("Budget not found");
            }

            if (budget.Status != BudgetStatus.PendingApproval)
            {
                return Result.Failure("Budget is not pending approval");
            }

            // Approve budget
            budget.Approve(userId, dto.Comments);

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Budget {BudgetId} approved by {UserId}", id, userId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error approving budget {BudgetId}", id);
            return Result.Failure($"Failed to approve budget: {ex.Message}");
        }
    }

    public async Task<Result> RejectBudgetAsync(
        Guid id,
        RejectBudgetDto dto,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var budget = await _unitOfWork
                .Repository<Budget>()
                .GetAsync(b => b.Id == id && !b.IsDeleted, cancellationToken: cancellationToken);

            if (budget == null)
            {
                return Result.Failure("Budget not found");
            }

            if (budget.Status != BudgetStatus.PendingApproval)
            {
                return Result.Failure("Budget is not pending approval");
            }

            // Reject budget
            budget.Reject(userId, dto.Reason);

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Budget {BudgetId} rejected by {UserId}", id, userId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error rejecting budget {BudgetId}", id);
            return Result.Failure($"Failed to reject budget: {ex.Message}");
        }
    }

    public async Task<Result> SetBudgetAsBaselineAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var budget = await _unitOfWork
                .Repository<Budget>()
                .GetAsync(b => b.Id == id && !b.IsDeleted, cancellationToken: cancellationToken);

            if (budget == null)
            {
                return Result.Failure("Budget not found");
            }

            if (budget.Status != BudgetStatus.Approved)
            {
                return Result.Failure("Only approved budgets can be set as baseline");
            }

            // Remove baseline status from other budgets in the project
            var existingBaselines = await _unitOfWork
                .Repository<Budget>()
                .GetAllAsync(
                    filter: b =>
                        b.ProjectId == budget.ProjectId
                        && b.IsBaseline
                        && b.Id != id
                        && !b.IsDeleted,
                    cancellationToken: cancellationToken
                );

            foreach (var baseline in existingBaselines)
            {
                baseline.RemoveBaselineStatus(userId);
                _unitOfWork.Repository<Budget>().Update(baseline);
            }

            // Set as baseline
            budget.SetAsBaseline(userId);

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Budget {BudgetId} set as baseline by {UserId}", id, userId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error setting budget {BudgetId} as baseline", id);
            return Result.Failure($"Failed to set budget as baseline: {ex.Message}");
        }
    }

    public async Task<Result> LockBudgetAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var budget = await _unitOfWork
                .Repository<Budget>()
                .GetAsync(b => b.Id == id && !b.IsDeleted, cancellationToken: cancellationToken);

            if (budget == null)
            {
                return Result.Failure("Budget not found");
            }

            if (budget.Status != BudgetStatus.Approved && budget.Status != BudgetStatus.Baseline)
            {
                return Result.Failure("Only approved or baseline budgets can be locked");
            }

            budget.Lock(userId);

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget {BudgetId} locked by {UserId}", id, userId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking budget {BudgetId}", id);
            return Result.Failure($"Failed to lock budget: {ex.Message}");
        }
    }

    #endregion

    #region Budget Item Operations

    public async Task<Result<Guid>> AddBudgetItemAsync(
        CreateBudgetItemDto dto,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var budget = await _unitOfWork
                .Repository<Budget>()
                .GetAsync(
                    filter: b => b.Id == dto.BudgetId && !b.IsDeleted,
                    includeProperties: "BudgetItems",
                    cancellationToken: cancellationToken
                );

            if (budget == null)
            {
                return Result<Guid>.Failure("Budget not found");
            }

            if (budget.Status != BudgetStatus.Draft)
            {
                return Result<Guid>.Failure("Items can only be added to draft budgets");
            }

            // Validate unique item code within budget
            if (budget.BudgetItems.Any(i => i.ItemCode == dto.ItemCode && !i.IsDeleted))
            {
                return Result<Guid>.Failure(
                    $"Item with code '{dto.ItemCode}' already exists in this budget"
                );
            }

            // Create budget item
            var item = budget.AddBudgetItem(
                dto.ItemCode,
                dto.Description,
                dto.CostType, dto.Category, dto.Quantity, dto.UnitRate, dto.ControlAccountId, dto.UnitOfMeasure, dto.AccountingCode, dto.Notes, userId);

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Added item {ItemCode} to budget {BudgetId}",
                dto.ItemCode,
                dto.BudgetId
            );

            return Result<Guid>.Success(item.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error adding item to budget {BudgetId}", dto.BudgetId);
            return Result<Guid>.Failure($"Failed to add budget item: {ex.Message}");
        }
    }

    public async Task<Result> UpdateBudgetItemAsync(
        Guid itemId,
        decimal quantity,
        decimal unitRate,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var item = await _unitOfWork
                .Repository<BudgetItem>()
                .GetAsync(
                    filter: i => i.Id == itemId && !i.IsDeleted,
                    includeProperties: "Budget",
                    cancellationToken: cancellationToken
                );

            if (item == null)
            {
                return Result.Failure("Budget item not found");
            }

            if (item.Budget.Status != BudgetStatus.Draft)
            {
                return Result.Failure("Items can only be updated in draft budgets");
            }

            // Update item
            item.UpdateAmount(quantity, unitRate, userId);

            _unitOfWork.Repository<BudgetItem>().Update(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Updated budget item {ItemId}", itemId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error updating budget item {ItemId}", itemId);
            return Result.Failure($"Failed to update budget item: {ex.Message}");
        }
    }

    public async Task<Result> RemoveBudgetItemAsync(
        Guid itemId,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var item = await _unitOfWork
                .Repository<BudgetItem>()
                .GetAsync(
                    filter: i => i.Id == itemId && !i.IsDeleted,
                    includeProperties: "Budget",
                    cancellationToken: cancellationToken
                );

            if (item == null)
            {
                return Result.Failure("Budget item not found");
            }

            if (item.Budget.Status != BudgetStatus.Draft)
            {
                return Result.Failure("Items can only be removed from draft budgets");
            }

            // Soft delete
            item.SoftDelete(userId);

            _unitOfWork.Repository<BudgetItem>().Update(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Removed budget item {ItemId}", itemId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error removing budget item {ItemId}", itemId);
            return Result.Failure($"Failed to remove budget item: {ex.Message}");
        }
    }

    #endregion

    #region Revision Operations

    public async Task<Result<Guid>> CreateBudgetRevisionAsync(
        Guid budgetId,
        CreateBudgetRevisionDto dto,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var budget = await _unitOfWork
                .Repository<Budget>()
                .GetAsync(
                    filter: b => b.Id == budgetId && !b.IsDeleted,
                    includeProperties: "BudgetItems,Revisions",
                    cancellationToken: cancellationToken
                );

            if (budget == null)
            {
                return Result<Guid>.Failure("Budget not found");
            }

            if (budget.Status != BudgetStatus.Approved && budget.Status != BudgetStatus.Baseline)
            {
                return Result<Guid>.Failure("Only approved or baseline budgets can be revised");
            }

            // Create revision
            var revision = budget.CreateRevisionRecord(dto.RevisionNumber, dto.Reason, userId, dto.Description);


            // Create new budget from revision

            var newBudget = revision.Budget;

            await _unitOfWork.Repository<Budget>().AddAsync(newBudget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Created revision {RevisionNumber} for budget {BudgetId}",
                dto.RevisionNumber,
                budgetId
            );

            return Result<Guid>.Success(newBudget.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error creating budget revision for budget {BudgetId}", budgetId);
            return Result<Guid>.Failure($"Failed to create budget revision: {ex.Message}");
        }
    }

    public async Task<Result> ApproveBudgetRevisionAsync(
        Guid revisionId,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var revision = await _unitOfWork
                .Repository<BudgetRevision>()
                .GetAsync(
                    filter: r => r.Id == revisionId,
                    includeProperties: "Budget",
                    cancellationToken: cancellationToken
                );

            if (revision == null)
            {
                return Result.Failure("Budget revision not found");
            }

            if (revision.IsApproved)
            {
                return Result.Failure("Revision is already approved");
            }

            // Approve revision
            revision.Approve(userId);

            _unitOfWork.Repository<BudgetRevision>().Update(revision);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Budget revision {RevisionId} approved by {UserId}",
                revisionId,
                userId
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error approving budget revision {RevisionId}", revisionId);
            return Result.Failure($"Failed to approve budget revision: {ex.Message}");
        }
    }

    #endregion

    #region Private Methods

    private async Task<Result> ValidateBudgetForApproval(Budget budget)
    {
        // Check if budget has items
        if (!budget.BudgetItems.Any(i => !i.IsDeleted))
        {
            return Result.Failure("Budget must have at least one item before approval");
        }

        // Check if total allocated matches total budget (within tolerance)
        var totalAllocated = budget.BudgetItems.Where(i => !i.IsDeleted).Sum(i => i.Amount);

        var expectedTotal =
            budget.TotalAmount - budget.ContingencyAmount - budget.ManagementReserve;
        var tolerance = expectedTotal * 0.01m; // 1% tolerance

        if (Math.Abs(totalAllocated - expectedTotal) > tolerance)
        {
            return Result.Failure(
                $"Total allocated ({totalAllocated:C}) does not match expected total ({expectedTotal:C})"
            );
        }

        // Additional validation rules can be added here

        return Result.Success();
    }

    private IQueryable<Budget> ApplySorting(
        IQueryable<Budget> query,
        string? sortBy,
        bool sortDescending
    )
    {
        return sortBy?.ToLower() switch
        {
            "version" => sortDescending
                ? query.OrderByDescending(b => b.Version)
                : query.OrderBy(b => b.Version),
            "name" => sortDescending
                ? query.OrderByDescending(b => b.Name)
                : query.OrderBy(b => b.Name),
            "amount" => sortDescending
                ? query.OrderByDescending(b => b.TotalAmount)
                : query.OrderBy(b => b.TotalAmount),
            "status" => sortDescending
                ? query.OrderByDescending(b => b.Status)
                : query.OrderBy(b => b.Status),
            "created" => sortDescending
                ? query.OrderByDescending(b => b.CreatedAt)
                : query.OrderBy(b => b.CreatedAt),
            _ => query.OrderByDescending(b => b.CreatedAt),
        };
    }

    protected override Expression<Func<Budget, bool>> GetBaseFilter()
    {
        return b => !b.IsDeleted;
    }

    #endregion
}
