using Application.Interfaces.Cost;
using Application.Interfaces.Common;
using Application.Interfaces.Auth;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Cost;
using Core.DTOs.Cost.CostControlReports;
using Core.DTOs.Cost.CostItems;
using Core.DTOs.Cost.PlanningPackages;
using Domain.Entities.Cost.Control;
using Domain.Entities.Organization.Core;
using Microsoft.EntityFrameworkCore;
using Core.Enums.Cost;
using Core.Enums.Progress;
using Core.Enums.Projects;
using Domain.Entities.WBS;

namespace Application.Services.Cost;

/// <summary>
/// Service for managing costs and planning packages
/// </summary>
public class CostService : ICostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CostService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    #region Cost Item Operations

    public async Task<PagedResult<CostItemDto>> GetCostItemsAsync(
        Guid projectId,
        CostQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<CostItem>()
            .Query()
            .Where(c => c.ProjectId == projectId && !c.IsDeleted);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(c =>
                c.ItemCode.ToLower().Contains(searchTerm) ||
                c.Description.ToLower().Contains(searchTerm) ||
                (c.ReferenceNumber != null && c.ReferenceNumber.ToLower().Contains(searchTerm)));
        }

        // Apply filters
        if (parameters.Type.HasValue)
            query = query.Where(c => c.Type == parameters.Type.Value);
        
        if (parameters.Category.HasValue)
            query = query.Where(c => c.Category == parameters.Category.Value);
        
        if (parameters.Status.HasValue)
            query = query.Where(c => c.Status == parameters.Status.Value);
        
        if (parameters.ControlAccountId.HasValue)
            query = query.Where(c => c.ControlAccountId == parameters.ControlAccountId.Value);
        
        if (parameters.WBSElementId.HasValue)
            query = query.Where(c => c.WBSElementId == parameters.WBSElementId.Value);
        
        if (parameters.IsApproved.HasValue)
            query = query.Where(c => c.IsApproved == parameters.IsApproved.Value);

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            query = parameters.SortBy.ToLower() switch
            {
                "itemcode" => parameters.IsAscending
                    ? query.OrderBy(c => c.ItemCode)
                    : query.OrderByDescending(c => c.ItemCode),
                "description" => parameters.IsAscending
                    ? query.OrderBy(c => c.Description)
                    : query.OrderByDescending(c => c.Description),
                "plannedcost" => parameters.IsAscending
                    ? query.OrderBy(c => c.PlannedCost)
                    : query.OrderByDescending(c => c.PlannedCost),
                "actualcost" => parameters.IsAscending
                    ? query.OrderBy(c => c.ActualCost)
                    : query.OrderByDescending(c => c.ActualCost),
                "variance" => parameters.IsAscending
                    ? query.OrderBy(c => c.Variance)
                    : query.OrderByDescending(c => c.Variance),
                "createdat" => parameters.IsAscending
                    ? query.OrderBy(c => c.CreatedAt)
                    : query.OrderByDescending(c => c.CreatedAt),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(c => c.CreatedAt);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(c => c.Project)
            .Include(c => c.WBSElement)
            .Include(c => c.ControlAccount)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<CostItemDto>>(items);

        var totalPages = parameters.PageSize > 0 ? (int)Math.Ceiling(totalCount / (double)parameters.PageSize) : 0;
        return new PagedResult<CostItemDto>
        {
            Items = dtos,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalPages = totalPages
        };
    }

    public async Task<CostItemDetailDto?> GetCostItemByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<CostItem>()
            .GetAsync(
                filter: c => c.Id == id && !c.IsDeleted,
                includeProperties: "Project,WBSElement,ControlAccount,CBS",
                cancellationToken: cancellationToken);

        if (entity == null)
            return null;

        return _mapper.Map<CostItemDetailDto>(entity);
    }

    public async Task<ProjectCostReportDto> GetProjectCostReportAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        var effectiveDate = asOfDate ?? DateTime.UtcNow;
        
        var costItems = await _unitOfWork.Repository<CostItem>()
            .GetAllAsync(
                filter: c => c.ProjectId == projectId && 
                           !c.IsDeleted && 
                           c.CreatedAt <= effectiveDate,
                cancellationToken: cancellationToken);

        var project = await _unitOfWork.Repository<Project>()
            .GetAsync(
                filter: p => p.Id == projectId,
                cancellationToken: cancellationToken);

        if (project == null)
            throw new InvalidOperationException("Project not found");

        var report = new ProjectCostReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            ReportDate = effectiveDate,
            Currency = project.Currency,
            
            // Summary
            TotalPlannedCost = costItems.Sum(c => c.PlannedCost),
            TotalActualCost = costItems.Sum(c => c.ActualCost),
            TotalCommittedCost = costItems.Sum(c => c.CommittedCost),
            TotalForecastCost = costItems.Sum(c => c.ForecastCost),
            
            // TODO: Get budget from project or control accounts
            TotalBudget = 0, // Would need to be calculated from project budget
            ContingencyReserve = 0, // Would need to be stored in project
            ContingencyUsed = 0, // Would need to be calculated
            ManagementReserve = 0, // Would need to be stored in project
            
            // Control Account breakdown
            ControlAccountsBreakdown = await GetCostSummaryByControlAccountAsync(projectId, cancellationToken),
            
            // Cost trends - simplified for now
            CostTrends = new List<CostTrendDto>()
        };

        // Calculate variances
        report.TotalCostVariance = report.TotalActualCost - report.TotalPlannedCost;
        report.TotalCostVariancePercentage = report.TotalPlannedCost > 0 
            ? report.TotalCostVariance / report.TotalPlannedCost * 100 : 0;

        return report;
    }

    public async Task<List<CostSummaryByCategoryDto>> GetCostSummaryByCategoryAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var costItems = await _unitOfWork.Repository<CostItem>()
            .Query()
            .Where(c => c.ProjectId == projectId && !c.IsDeleted)
            .GroupBy(c => c.Category)
            .Select(g => new CostSummaryByCategoryDto
            {
                Category = g.Key,
                CategoryName = g.Key.ToString(),
                ItemCount = g.Count(),
                PlannedCost = g.Sum(c => c.PlannedCost),
                ActualCost = g.Sum(c => c.ActualCost),
                CommittedCost = g.Sum(c => c.CommittedCost),
                ForecastCost = g.Sum(c => c.ForecastCost),
                Variance = g.Sum(c => c.ActualCost - c.PlannedCost),
                VariancePercentage = g.Sum(c => c.PlannedCost) > 0 
                    ? (g.Sum(c => c.ActualCost - c.PlannedCost) / g.Sum(c => c.PlannedCost)) * 100 : 0
            })
            .ToListAsync(cancellationToken);

        return costItems;
    }

    public async Task<List<CostSummaryByControlAccountDto>> GetCostSummaryByControlAccountAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var costItems = await _unitOfWork.Repository<CostItem>()
            .Query()
            .Where(c => c.ProjectId == projectId && !c.IsDeleted && c.ControlAccountId.HasValue)
            .Include(c => c.ControlAccount)
            .GroupBy(c => new { c.ControlAccountId, c.ControlAccount!.Code, c.ControlAccount.Name })
            .Select(g => new CostSummaryByControlAccountDto
            {
                ControlAccountId = g.Key.ControlAccountId!.Value,
                ControlAccountCode = g.Key.Code,
                ControlAccountName = g.Key.Name,
                ItemCount = g.Count(),
                PlannedCost = g.Sum(c => c.PlannedCost),
                ActualCost = g.Sum(c => c.ActualCost),
                CommittedCost = g.Sum(c => c.CommittedCost),
                ForecastCost = g.Sum(c => c.ForecastCost),
                Variance = g.Sum(c => c.ActualCost - c.PlannedCost),
                VariancePercentage = g.Sum(c => c.PlannedCost) > 0 
                    ? (g.Sum(c => c.ActualCost - c.PlannedCost) / g.Sum(c => c.PlannedCost)) * 100 : 0
            })
            .ToListAsync(cancellationToken);

        return costItems;
    }

    public async Task<Result<Guid>> CreateCostItemAsync(
        CreateCostItemDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate project exists
            var project = await _unitOfWork.Repository<Project>()
                .GetAsync(
                    filter: p => p.Id == dto.ProjectId && !p.IsDeleted,
                    cancellationToken: cancellationToken);

            if (project == null)
                return Result<Guid>.Failure("Project not found");

            // Validate control account if provided
            if (dto.ControlAccountId.HasValue)
            {
                var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                    .GetAsync(
                        filter: ca => ca.Id == dto.ControlAccountId.Value && 
                                     ca.ProjectId == dto.ProjectId && 
                                     !ca.IsDeleted,
                        cancellationToken: cancellationToken);

                if (controlAccount == null)
                    return Result<Guid>.Failure("Control Account not found");
            }

            // Create cost item
            var costItem = new CostItem(
                dto.ProjectId,
                dto.ItemCode,
                dto.Description,
                dto.Type,
                dto.Category,
                dto.PlannedCost);

            costItem.CreatedBy = userId;

            if (dto.ControlAccountId.HasValue)
                costItem.AssignToControlAccount(dto.ControlAccountId.Value);

            if (dto.WorkPackageId.HasValue)
                costItem.AssignToWBSElement(dto.WorkPackageId.Value);

            await _unitOfWork.Repository<CostItem>().AddAsync(costItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(costItem.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create cost item: {ex.Message}");
        }
    }

    public async Task<Result> UpdateCostItemAsync(
        Guid id,
        UpdateCostItemDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var costItem = await _unitOfWork.Repository<CostItem>()
                .GetAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            if (costItem == null)
                return Result.Failure("Cost item not found");

            // Update metadata
            costItem.UpdatedBy = userId;
            costItem.UpdatedAt = DateTime.UtcNow;

            // Update planned cost if provided
            if (dto.PlannedCost.HasValue)
                costItem.UpdatePlannedCost(dto.PlannedCost.Value);

            _unitOfWork.Repository<CostItem>().Update(costItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update cost item: {ex.Message}");
        }
    }

    public async Task<Result> RecordActualCostAsync(
        Guid costItemId,
        RecordActualCostDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var costItem = await _unitOfWork.Repository<CostItem>()
                .GetAsync(
                    filter: c => c.Id == costItemId && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            if (costItem == null)
                return Result.Failure("Cost item not found");

            costItem.RecordActualCost(
                dto.ActualCost,
                dto.TransactionDate,
                dto.ReferenceType,
                dto.ReferenceNumber);

            costItem.UpdatedBy = userId;

            _unitOfWork.Repository<CostItem>().Update(costItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to record actual cost: {ex.Message}");
        }
    }

    public async Task<Result> RecordCommitmentAsync(
        Guid costItemId,
        RecordCommitmentDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var costItem = await _unitOfWork.Repository<CostItem>()
                .GetAsync(
                    filter: c => c.Id == costItemId && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            if (costItem == null)
                return Result.Failure("Cost item not found");

            costItem.RecordCommitment(
                dto.CommittedCost,
                dto.ReferenceType,
                dto.ReferenceNumber);

            costItem.UpdatedBy = userId;

            _unitOfWork.Repository<CostItem>().Update(costItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to record commitment: {ex.Message}");
        }
    }

    public async Task<Result> ApproveCostItemAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var costItem = await _unitOfWork.Repository<CostItem>()
                .GetAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            if (costItem == null)
                return Result.Failure("Cost item not found");

            if (costItem.IsApproved)
                return Result.Failure("Cost item is already approved");

            costItem.Approve(userId);

            _unitOfWork.Repository<CostItem>().Update(costItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to approve cost item: {ex.Message}");
        }
    }

    public async Task<Result> DeleteCostItemAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var costItem = await _unitOfWork.Repository<CostItem>()
                .GetAsync(
                    filter: c => c.Id == id && !c.IsDeleted,
                    cancellationToken: cancellationToken);

            if (costItem == null)
                return Result.Failure("Cost item not found");

            if (costItem.ActualCost > 0)
                return Result.Failure("Cannot delete cost item with actual costs recorded");

            costItem.IsDeleted = true;
            costItem.DeletedAt = DateTime.UtcNow;
            costItem.DeletedBy = userId;

            _unitOfWork.Repository<CostItem>().Update(costItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete cost item: {ex.Message}");
        }
    }

    #endregion

    #region Planning Package Operations

    public async Task<PagedResult<PlanningPackageDto>> GetPlanningPackagesAsync(
        Guid controlAccountId,
        PlanningPackageQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<PlanningPackage>()
            .Query()
            .Where(pp => pp.ControlAccountId == controlAccountId && !pp.IsDeleted);

        // Apply search
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(pp =>
                pp.Code.ToLower().Contains(searchTerm) ||
                pp.Name.ToLower().Contains(searchTerm) ||
                (pp.Description != null && pp.Description.ToLower().Contains(searchTerm)));
        }

        // Apply filters
        if (parameters.Status.HasValue)
            query = query.Where(pp => pp.Status == parameters.Status.Value);
        
        if (parameters.IsConverted.HasValue)
            query = query.Where(pp => pp.IsConverted == parameters.IsConverted.Value);
        
        if (parameters.PhaseId.HasValue)
            query = query.Where(pp => pp.PhaseId == parameters.PhaseId.Value);

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            query = parameters.SortBy.ToLower() switch
            {
                "code" => parameters.IsAscending
                    ? query.OrderBy(pp => pp.Code)
                    : query.OrderByDescending(pp => pp.Code),
                "name" => parameters.IsAscending
                    ? query.OrderBy(pp => pp.Name)
                    : query.OrderByDescending(pp => pp.Name),
                "plannedstartdate" => parameters.IsAscending
                    ? query.OrderBy(pp => pp.PlannedStartDate)
                    : query.OrderByDescending(pp => pp.PlannedStartDate),
                "estimatedbudget" => parameters.IsAscending
                    ? query.OrderBy(pp => pp.EstimatedBudget)
                    : query.OrderByDescending(pp => pp.EstimatedBudget),
                "priority" => parameters.IsAscending
                    ? query.OrderBy(pp => pp.Priority)
                    : query.OrderByDescending(pp => pp.Priority),
                "plannedconversiondate" => parameters.IsAscending
                    ? query.OrderBy(pp => pp.PlannedConversionDate)
                    : query.OrderByDescending(pp => pp.PlannedConversionDate),
                _ => query.OrderBy(pp => pp.Priority).ThenBy(pp => pp.PlannedConversionDate)
            };
        }
        else
        {
            query = query.OrderBy(pp => pp.Priority).ThenBy(pp => pp.PlannedConversionDate);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(pp => pp.ControlAccount)
            .Include(pp => pp.Phase)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<PlanningPackageDto>>(items);

        var totalPages = parameters.PageSize > 0 ? (int)Math.Ceiling(totalCount / (double)parameters.PageSize) : 0;
        return new PagedResult<PlanningPackageDto>
        {
            Items = dtos,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalPages = totalPages
        };
    }

    public async Task<Result<Guid>> CreatePlanningPackageAsync(
        CreatePlanningPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate control account exists
            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .GetAsync(
                    filter: ca => ca.Id == dto.ControlAccountId && !ca.IsDeleted,
                    cancellationToken: cancellationToken);

            if (controlAccount == null)
                return Result<Guid>.Failure("Control Account not found");

            // Get default phase - assuming first active phase
            var phase = await _unitOfWork.Repository<Phase>()
                .Query()
                .Where(p => p.ProjectId == controlAccount.ProjectId && p.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (phase == null)
                return Result<Guid>.Failure("No active phase found for the project");

            // Create planning package
            var planningPackage = new PlanningPackage(
                dto.Code,
                dto.Name,
                dto.ControlAccountId,
                controlAccount.ProjectId,
                phase.Id,
                dto.PlannedStartDate,
                dto.PlannedEndDate,
                dto.Budget);

            planningPackage.CreatedBy = userId;

            await _unitOfWork.Repository<PlanningPackage>().AddAsync(planningPackage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(planningPackage.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create planning package: {ex.Message}");
        }
    }

    public async Task<Result> UpdatePlanningPackageAsync(
        Guid id,
        UpdatePlanningPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var planningPackage = await _unitOfWork.Repository<PlanningPackage>()
                .GetAsync(
                    filter: pp => pp.Id == id && !pp.IsDeleted,
                    cancellationToken: cancellationToken);

            if (planningPackage == null)
                return Result.Failure("Planning Package not found");

            if (planningPackage.IsConverted)
                return Result.Failure("Cannot update a converted Planning Package");

            // Update details if provided
            if (!string.IsNullOrWhiteSpace(dto.Name) || dto.Budget.HasValue)
            {
                planningPackage.UpdateDetails(
                    dto.Name ?? planningPackage.Name,
                    dto.Description,
                    dto.Budget ?? planningPackage.EstimatedBudget,
                    planningPackage.EstimatedHours, // Keep existing hours
                    userId);
            }

            // Update schedule if dates provided
            if (dto.PlannedStartDate.HasValue && dto.PlannedEndDate.HasValue)
            {
                planningPackage.UpdateSchedule(
                    dto.PlannedStartDate.Value,
                    dto.PlannedEndDate.Value,
                    planningPackage.PlannedConversionDate, // Keep existing conversion date
                    userId);
            }

            _unitOfWork.Repository<PlanningPackage>().Update(planningPackage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update planning package: {ex.Message}");
        }
    }

    public async Task<Result> ConvertPlanningPackageToWorkPackagesAsync(
        Guid id,
        ConvertPlanningPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var planningPackage = await _unitOfWork.Repository<PlanningPackage>()
                .GetAsync(
                    filter: pp => pp.Id == id && !pp.IsDeleted,
                    cancellationToken: cancellationToken);

            if (planningPackage == null)
                return Result.Failure("Planning Package not found");

            if (planningPackage.IsConverted)
                return Result.Failure("Planning Package has already been converted");

            if (!planningPackage.IsReadyForConversion())
                return Result.Failure("Planning Package is not ready for conversion");

            // Mark as converted
            planningPackage.ConvertToWorkPackage(userId);

            // Create work packages based on the conversion details
            // This would involve creating WorkPackageDetails entities based on dto.WorkPackages
            // Implementation depends on the WorkPackageDetails structure

            _unitOfWork.Repository<PlanningPackage>().Update(planningPackage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to convert planning package: {ex.Message}");
        }
    }

    #endregion
}