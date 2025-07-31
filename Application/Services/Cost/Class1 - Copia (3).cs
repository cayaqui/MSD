using Application.Interfaces.Auth;
using Application.Interfaces.Cost;
using Core.DTOs.Budget;
using Core.Enums.Cost;
using Domain.Entities.Cost;
using Domain.Entities.Projects;
using Microsoft.Graph.Communications.Calls.Item.SendDtmfTones;
using InvalidOperationException = Application.Common.Exceptions.InvalidOperationException;

namespace Application.Services.Cost;

/// <summary>
/// Implementation of Budget service operations
/// </summary>
public class BudgetService : IBudgetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogger<BudgetService> _logger;

    public BudgetService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserContext currentUser,
        ILogger<BudgetService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    #region Query Operations

    public async Task<PagedResult<BudgetDto>> GetBudgetsAsync(
        QueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _unitOfWork.Repository<Budget>()
                .Query()
                .Include(b => b.Project)
                .Include(b => b.CreatedBy)
                .Include(b => b.ApprovedBy)
                .Where(b => !b.IsDeleted);

            // Apply filters
            if (parameters.Filters.TryGetValue("projectId", out var projectId))
            {
                if (Guid.TryParse(projectId, out var projectGuid))
                {
                    query = query.Where(b => b.ProjectId == projectGuid);
                }
            }

            if (parameters.Filters.TryGetValue("type", out var type))
            {
                if (Enum.TryParse<BudgetType>(type, out var typeEnum))
                {
                    query = query.Where(b => b.Type == typeEnum);
                }
            }

            if (parameters.Filters.TryGetValue("status", out var status))
            {
                if (Enum.TryParse<BudgetStatus>(status, out var statusEnum))
                {
                    query = query.Where(b => b.Status == statusEnum);
                }
            }

            if (parameters.Filters.TryGetValue("isBaseline", out var isBaseline))
            {
                if (bool.TryParse(isBaseline, out var isBaselineBool))
                {
                    query = query.Where(b => b.IsBaseline == isBaselineBool);
                }
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchLower = parameters.SearchTerm.ToLower();
                query = query.Where(b =>
                    b.Version.ToLower().Contains(searchLower) ||
                    b.Name.ToLower().Contains(searchLower) ||
                    (b.Description != null && b.Description.ToLower().Contains(searchLower)));
            }

            // Apply sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "version" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(b => b.Version)
                    : query.OrderBy(b => b.Version),
                "name" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(b => b.Name)
                    : query.OrderBy(b => b.Name),
                "totalamount" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(b => b.TotalAmount)
                    : query.OrderBy(b => b.TotalAmount),
                "status" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(b => b.Status)
                    : query.OrderBy(b => b.Status),
                "createdat" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(b => b.CreatedAt)
                    : query.OrderBy(b => b.CreatedAt),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<BudgetDto>>(items);

            return new PagedResult<BudgetDto>
            {
                Items = dtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budgets");
            throw;
        }
    }

    public async Task<BudgetDetailDto?> GetBudgetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var budget = await _unitOfWork.Repository<Budget>()
                .Query()
                .Include(b => b.Project)
                .Include(b => b.BudgetItems)
                    .ThenInclude(bi => bi.ControlAccount)
                .Include(b => b.CreatedBy)
                .Include(b => b.ApprovedBy)
                .Include(b => b.Revisions)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, cancellationToken);

            if (budget == null)
                return null;

            var dto = _mapper.Map<BudgetDetailDto>(budget);

            // Calculate summary values
            dto.AllocatedAmount = budget.BudgetItems.Sum(bi => bi.Amount);
            dto.UnallocatedAmount = budget.TotalAmount - dto.AllocatedAmount;
            dto.ItemCount = budget.BudgetItems.Count;

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget by id: {Id}", id);
            throw;
        }
    }

    public async Task<BudgetDetailDto?> GetCurrentBaselineBudgetAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var budget = await _unitOfWork.Repository<Budget>()
                .Query()
                .Include(b => b.Project)
                .Include(b => b.BudgetItems)
                    .ThenInclude(bi => bi.ControlAccount)
                .Include(b => b.CreatedBy)
                .Include(b => b.ApprovedBy)
                .Where(b => b.ProjectId == projectId &&
                           b.IsBaseline &&
                           b.Status == BudgetStatus.Approved &&
                           !b.IsDeleted)
                .OrderByDescending(b => b.BaselineDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (budget == null)
                return null;

            return _mapper.Map<BudgetDetailDto>(budget);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current baseline budget for project: {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<List<BudgetItemDto>> GetBudgetItemsAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _unitOfWork.Repository<BudgetItem>()
                .Query()
                .Include(bi => bi.ControlAccount)
                .Where(bi => bi.BudgetId == budgetId)
                .OrderBy(bi => bi.ItemCode)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<BudgetItemDto>>(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget items for budget: {BudgetId}", budgetId);
            throw;
        }
    }

    public async Task<List<BudgetVersionDto>> GetBudgetVersionsAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var budgets = await _unitOfWork.Repository<Budget>()
                .Query()
                .Where(b => b.ProjectId == projectId && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<BudgetVersionDto>>(budgets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget versions for project: {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<BudgetComparisonDto> CompareBudgetsAsync(
        Guid budgetId1,
        Guid budgetId2,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var budget1 = await GetBudgetByIdAsync(budgetId1, cancellationToken);
            var budget2 = await GetBudgetByIdAsync(budgetId2, cancellationToken);

            if (budget1 == null || budget2 == null)
                throw new InvalidOperationException("One or both budgets not found");

            var comparison = new BudgetComparisonDto
            {
                Budget1 = budget1,
                Budget2 = budget2,
                TotalVariance = budget2.TotalAmount - budget1.TotalAmount,
                PercentageVariance = budget1.TotalAmount > 0
                    ? ((budget2.TotalAmount - budget1.TotalAmount) / budget1.TotalAmount) * 100
                    : 0
            };

            // Compare budget items
            var items1Dict = budget1.BudgetItems.ToDictionary(i => i.ItemCode);
            var items2Dict = budget2.BudgetItems.ToDictionary(i => i.ItemCode);
            var allCodes = items1Dict.Keys.Union(items2Dict.Keys).OrderBy(c => c);

            foreach (var code in allCodes)
            {
                var item1 = items1Dict.TryGetValue(code, out var i1) ? i1 : null;
                var item2 = items2Dict.TryGetValue(code, out var i2) ? i2 : null;

                var itemComparison = new BudgetItemComparisonDto
                {
                    Code = code,
                    Description = item1?.Description ?? item2?.Description ?? "",
                    Amount1 = item1?.Amount ?? 0,
                    Amount2 = item2?.Amount ?? 0
                };

                itemComparison.Variance = itemComparison.Amount2 - itemComparison.Amount1;
                itemComparison.PercentageVariance = itemComparison.Amount1 > 0
                    ? (itemComparison.Variance / itemComparison.Amount1) * 100
                    : 0;

                comparison.ItemComparisons.Add(itemComparison);
            }

            return comparison;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing budgets: {BudgetId1} and {BudgetId2}", budgetId1, budgetId2);
            throw;
        }
    }

    #endregion

    #region Command Operations

    public async Task<Result<Guid>> CreateBudgetAsync(
        CreateBudgetDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating budget version: {Version} for project: {ProjectId} by user: {UserId}",
                dto.Version, dto.ProjectId, userId);

            // Validate project exists
            var project = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(dto.ProjectId, cancellationToken);
            if (project == null)
                return Result<Guid>.Failure("Project not found");

            // Check if version already exists
            var versionExists = await _unitOfWork.Repository<Budget>()
                .AnyAsync(b => b.ProjectId == dto.ProjectId &&
                              b.Version == dto.Version &&
                              !b.IsDeleted, cancellationToken);

            if (versionExists)
                return Result<Guid>.Failure($"Budget version '{dto.Version}' already exists for this project");

            // Create budget
            var budget = new Budget(
                dto.ProjectId,
                dto.Version,
                dto.Name,
                dto.Type,
                dto.TotalAmount,
                dto.Currency);

            if (!string.IsNullOrWhiteSpace(dto.Description))
            { budget.UpdateBasicInfo(dto.Name, dto.Description); }

            budget.UpdateFinancials(dto.TotalAmount, dto.ContingencyAmount, dto.ManagementReserve);


            budget.CreatedBy = userId;

            await _unitOfWork.Repository<Budget>().AddAsync(budget, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget created successfully: {Version} with Id: {Id}", dto.Version, budget.Id);

            return Result<Guid>.Success(budget.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating budget");
            return Result<Guid>.Failure($"Error creating budget: {ex.Message}");
        }
    }

    public async Task<Result> UpdateBudgetAsync(
        Guid id,
        UpdateBudgetDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating budget: {Id} by user: {UserId}", id, userId);

            var budget = await _unitOfWork.Repository<Budget>()
                .GetByIdAsync(id, cancellationToken);

            if (budget == null || budget.IsDeleted)
                return Result.Failure("Budget not found");

            if (budget.Status == BudgetStatus.Approved || budget.Status == BudgetStatus.UnderReview)
                return Result.Failure("Cannot update an approved or active budget");

            // Update basic info
            budget.UpdateBasicInfo(dto.Name, dto.Description);
            var totalAmount = dto.TotalAmount ?? 0;
            var contingencyAmount = dto.ContingencyAmount ?? 0;
            var managementReserve = dto.ManagementReserve ?? 0;
            var exchangeRate = dto.ExchangeRate ?? budget.ExchangeRate;
            budget.UpdateFinancials(totalAmount, contingencyAmount, managementReserve);


            if (dto.ExchangeRate.HasValue)
                budget.SetExchangeRate(dto.ExchangeRate.Value);

            budget.UpdatedBy = userId;
            budget.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget updated successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating budget: {Id}", id);
            return Result.Failure($"Error updating budget: {ex.Message}");
        }
    }

    public async Task<Result> AddBudgetItemAsync(
        Guid budgetId,
        CreateBudgetItemDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding budget item to budget: {BudgetId} by user: {UserId}", budgetId, userId);

            var budget = await _unitOfWork.Repository<Budget>()
                .Query()
                .Include(b => b.BudgetItems)
                .FirstOrDefaultAsync(b => b.Id == budgetId && !b.IsDeleted, cancellationToken);

            if (budget == null)
                return Result.Failure("Budget not found");

            if (budget.Status != BudgetStatus.Draft && budget.Status != BudgetStatus.UnderReview)
                return Result.Failure("Cannot add items to a budget that is not in draft or under review status");

            // Validate control account exists
            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .GetByIdAsync(dto.ControlAccountId?? Guid.Empty, cancellationToken);
            if (controlAccount == null)
                return Result.Failure("Control Account not found");

            // Check if code already exists in this budget
            var codeExists = budget.BudgetItems.Any(bi => bi.ItemCode == dto.ItemCode);
            if (codeExists)
                return Result.Failure($"Budget item with code '{dto.ItemCode}' already exists in this budget");

            // Create budget item
            var budgetItem = new BudgetItem(
                budgetId,
                dto.ItemCode, dto.Description, dto.CostType, dto.Category, 0);
            budgetItem.CreatedBy = userId;
            budgetItem.UpdateAmount(dto.Quantity, dto.UnitRate);
            await _unitOfWork.Repository<BudgetItem>().AddAsync(budgetItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget item added successfully to budget: {BudgetId}", budgetId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding budget item to budget: {BudgetId}", budgetId);
            return Result.Failure($"Error adding budget item: {ex.Message}");
        }
    }

    public async Task<Result> UpdateBudgetItemAsync(
        Guid budgetId,
        Guid itemId,
        UpdateBudgetItemDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating budget item: {ItemId} in budget: {BudgetId} by user: {UserId}",
                itemId, budgetId, userId);

            var budget = await _unitOfWork.Repository<Budget>()
                .GetByIdAsync(budgetId, cancellationToken);

            if (budget == null || budget.IsDeleted)
                return Result.Failure("Budget not found");

            if (budget.Status != BudgetStatus.Draft && budget.Status != BudgetStatus.UnderReview)
                return Result.Failure("Cannot update items in a budget that is not in draft or under review status");

            var budgetItem = await _unitOfWork.Repository<BudgetItem>()
                .FindSingleAsync(bi => bi.Id == itemId && bi.BudgetId == budgetId, cancellationToken);

            if (budgetItem == null)
                return Result.Failure("Budget item not found");
            budgetItem.UpdatedBy = userId;
            budgetItem.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<BudgetItem>().Update(budgetItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget item updated successfully: {ItemId}", itemId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating budget item: {ItemId}", itemId);
            return Result.Failure($"Error updating budget item: {ex.Message}");
        }
    }

    public async Task<Result> ApproveBudgetAsync(
        Guid id,
        ApproveBudgetDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving budget: {Id} by user: {UserId}", id, userId);

            var budget = await _unitOfWork.Repository<Budget>()
                .Query()
                .Include(b => b.BudgetItems)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, cancellationToken);

            if (budget == null)
                return Result.Failure("Budget not found");

            if (budget.Status != BudgetStatus.UnderReview)
                return Result.Failure("Budget must be under review to be approved");

            if (!budget.BudgetItems.Any())
                return Result.Failure("Cannot approve a budget with no items");

            budget.Approve(userId, dto.Comments);
            budget.UpdatedBy = userId;
            budget.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget approved successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving budget: {Id}", id);
            return Result.Failure($"Error approving budget: {ex.Message}");
        }
    }

    public async Task<Result> RejectBudgetAsync(
        Guid id,
        RejectBudgetDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Rejecting budget: {Id} by user: {UserId}", id, userId);

            var budget = await _unitOfWork.Repository<Budget>()
                .GetByIdAsync(id, cancellationToken);

            if (budget == null || budget.IsDeleted)
                return Result.Failure("Budget not found");

            if (budget.Status != BudgetStatus.UnderReview)
                return Result.Failure("Budget must be under review to be rejected");

            budget.Reject(userId, dto.Reason);
            budget.UpdatedBy = userId;
            budget.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget rejected successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting budget: {Id}", id);
            return Result.Failure($"Error rejecting budget: {ex.Message}");
        }
    }

    public async Task<Result> SetAsBaselineAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Setting budget as baseline: {Id} by user: {UserId}", id, userId);

            var budget = await _unitOfWork.Repository<Budget>()
                .GetByIdAsync(id, cancellationToken);

            if (budget == null || budget.IsDeleted)
                return Result.Failure("Budget not found");

            if (budget.Status != BudgetStatus.Approved)
                return Result.Failure("Only approved budgets can be set as baseline");

            // Deactivate current baseline if exists
            var currentBaseline = await _unitOfWork.Repository<Budget>()
                .FindSingleAsync(b => b.ProjectId == budget.ProjectId &&
                                     b.IsBaseline &&
                                     b.Status == BudgetStatus.Active &&
                                     !b.IsDeleted, cancellationToken);

            if (currentBaseline != null)
            {
                currentBaseline.RemoveBaseline();
                currentBaseline.UpdatedBy = userId;
                currentBaseline.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Budget>().Update(currentBaseline);
            }

            budget.SetAsBaseline();
            budget.UpdatedBy = userId;
            budget.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget set as baseline successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting budget as baseline: {Id}", id);
            return Result.Failure($"Error setting budget as baseline: {ex.Message}");
        }
    }

    public async Task<Result> CopyBudgetAsync(
        Guid id,
        string newVersion,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Copying budget: {Id} to new version: {Version} by user: {UserId}",
                id, newVersion, userId);

            var sourceBudget = await _unitOfWork.Repository<Budget>()
                .Query()
                .Include(b => b.BudgetItems)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, cancellationToken);

            if (sourceBudget == null)
                return Result.Failure("Source budget not found");

            // Check if new version already exists
            var versionExists = await _unitOfWork.Repository<Budget>()
                .AnyAsync(b => b.ProjectId == sourceBudget.ProjectId &&
                              b.Version == newVersion &&
                              !b.IsDeleted, cancellationToken);

            if (versionExists)
                return Result.Failure($"Budget version '{newVersion}' already exists for this project");

            // Create new budget
            var newBudget = new Budget(
                sourceBudget.ProjectId,
                newVersion,
                $"{sourceBudget.Name} (Copy)",
                sourceBudget.Type,
                sourceBudget.TotalAmount,
                sourceBudget.Currency);

            newBudget.UpdateDescription(sourceBudget.Description);
            newBudget.UpdateContingency(sourceBudget.ContingencyAmount);
            newBudget.UpdateManagementReserve(sourceBudget.ManagementReserve);
            newBudget.UpdateExchangeRate(sourceBudget.ExchangeRate);
            newBudget.CreatedBy = userId;

            await _unitOfWork.Repository<Budget>().AddAsync(newBudget, cancellationToken);

            // Copy budget items
            foreach (var sourceItem in sourceBudget.BudgetItems)
            {
                var newItem = new BudgetItem(
                    newBudget.Id,
                    sourceItem.ControlAccountId,
                    sourceItem.Code,
                    sourceItem.Description,
                    sourceItem.Amount,
                    sourceItem.Currency);

                newItem.CreatedBy = userId;
                await _unitOfWork.Repository<BudgetItem>().AddAsync(newItem, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget copied successfully from {SourceId} to new version {Version} with Id: {NewId}",
                id, newVersion, newBudget.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying budget: {Id}", id);
            return Result.Failure($"Error copying budget: {ex.Message}");
        }
    }

    public async Task<Result> DeleteBudgetAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting budget: {Id} by user: {UserId}", id, userId);

            var budget = await _unitOfWork.Repository<Budget>()
                .GetByIdAsync(id, cancellationToken);

            if (budget == null || budget.IsDeleted)
                return Result.Failure("Budget not found");

            if (budget.Status == BudgetStatus.Active || budget.IsBaseline)
                return Result.Failure("Cannot delete an active or baseline budget");

            budget.Delete(userId);

            _unitOfWork.Repository<Budget>().Update(budget);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget deleted successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting budget: {Id}", id);
            return Result.Failure($"Error deleting budget: {ex.Message}");
        }
    }

    public async Task<Result> DeleteBudgetItemAsync(
        Guid budgetId,
        Guid itemId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting budget item: {ItemId} from budget: {BudgetId} by user: {UserId}",
                itemId, budgetId, userId);

            var budget = await _unitOfWork.Repository<Budget>()
                .GetByIdAsync(budgetId, cancellationToken);

            if (budget == null || budget.IsDeleted)
                return Result.Failure("Budget not found");

            if (budget.Status != BudgetStatus.Draft && budget.Status != BudgetStatus.UnderReview)
                return Result.Failure("Cannot delete items from a budget that is not in draft or under review status");

            var budgetItem = await _unitOfWork.Repository<BudgetItem>()
                .FindSingleAsync(bi => bi.Id == itemId && bi.BudgetId == budgetId, cancellationToken);

            if (budgetItem == null)
                return Result.Failure("Budget item not found");

            _unitOfWork.Repository<BudgetItem>().Remove(budgetItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Budget item deleted successfully: {ItemId}", itemId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting budget item: {ItemId}", itemId);
            return Result.Failure($"Error deleting budget item: {ex.Message}");
        }
    }

    #endregion
}