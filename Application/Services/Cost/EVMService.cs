using Application.Interfaces.Cost;
using Application.Interfaces.Common;
using Application.Interfaces.Auth;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.EVM;
using Core.DTOs.Reports;
using Domain.Entities.Cost.EVM;
using Domain.Entities.Cost.Control;
using Domain.Entities.Organization.Core;
using Domain.Entities.Projects.WBS;
using Domain.Entities.Auth.Security;
using Microsoft.EntityFrameworkCore;
using Core.Enums.Cost;
using Core.Enums.Projects;
using System.Linq;

namespace Application.Services.Cost;

/// <summary>
/// Service for Earned Value Management operations
/// </summary>
public class EVMService : IEVMService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public EVMService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    #region Query Operations

    public async Task<PagedResult<EVMRecordDto>> GetEVMRecordsAsync(
        Guid controlAccountId,
        QueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<EVMRecord>()
            .Query()
            .Where(e => e.ControlAccountId == controlAccountId);

        // Apply search
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(e =>
                (e.Comments != null && e.Comments.ToLower().Contains(searchTerm)) ||
                e.DataDate.ToString().Contains(searchTerm));
        }

        // Apply filters
        if (parameters.Filters != null && parameters.Filters.Any())
        {
            foreach (var filter in parameters.Filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "periodtype":
                        if (Enum.TryParse<EVMPeriodType>(filter.Value, out var periodType))
                            query = query.Where(e => e.PeriodType == periodType);
                        break;
                    case "status":
                        if (Enum.TryParse<EVMStatus>(filter.Value, out var status))
                            query = query.Where(e => e.Status == status);
                        break;
                    case "isapproved":
                        if (bool.TryParse(filter.Value, out var isApproved))
                            query = query.Where(e => e.IsApproved == isApproved);
                        break;
                    case "year":
                        if (int.TryParse(filter.Value, out var year))
                            query = query.Where(e => e.Year == year);
                        break;
                    case "month":
                        if (int.TryParse(filter.Value, out var month))
                            query = query.Where(e => e.DataDate.Month == month);
                        break;
                }
            }
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            query = parameters.SortBy.ToLower() switch
            {
                "datadate" => parameters.IsAscending
                    ? query.OrderBy(e => e.DataDate)
                    : query.OrderByDescending(e => e.DataDate),
                "periodnumber" => parameters.IsAscending
                    ? query.OrderBy(e => e.PeriodNumber)
                    : query.OrderByDescending(e => e.PeriodNumber),
                "pv" => parameters.IsAscending
                    ? query.OrderBy(e => e.PV)
                    : query.OrderByDescending(e => e.PV),
                "ev" => parameters.IsAscending
                    ? query.OrderBy(e => e.EV)
                    : query.OrderByDescending(e => e.EV),
                "ac" => parameters.IsAscending
                    ? query.OrderBy(e => e.AC)
                    : query.OrderByDescending(e => e.AC),
                "cpi" => parameters.IsAscending
                    ? query.OrderBy(e => e.CPI)
                    : query.OrderByDescending(e => e.CPI),
                "spi" => parameters.IsAscending
                    ? query.OrderBy(e => e.SPI)
                    : query.OrderByDescending(e => e.SPI),
                _ => query.OrderByDescending(e => e.DataDate)
            };
        }
        else
        {
            query = query.OrderByDescending(e => e.DataDate);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(e => e.ControlAccount)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<EVMRecordDto>>(items);

        var totalPages = parameters.PageSize > 0 ? (int)Math.Ceiling(totalCount / (double)parameters.PageSize) : 0;
        return new PagedResult<EVMRecordDto>
        {
            Items = dtos,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalPages = totalPages
        };
    }

    public async Task<EVMRecordDetailDto?> GetEVMRecordByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<EVMRecord>()
            .GetAsync(
                filter: e => e.Id == id,
                includeProperties: "ControlAccount,ControlAccount.Project,ControlAccount.WBSElement",
                cancellationToken: cancellationToken);

        if (entity == null)
            return null;

        return _mapper.Map<EVMRecordDetailDto>(entity);
    }

    public async Task<EVMPerformanceReportDto> GetProjectEVMReportAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        var effectiveDate = asOfDate ?? DateTime.UtcNow;

        // Get project
        var project = await _unitOfWork.Repository<Project>()
            .GetAsync(
                filter: p => p.Id == projectId,
                cancellationToken: cancellationToken);

        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Get all control accounts for the project
        var controlAccounts = await _unitOfWork.Repository<ControlAccount>()
            .GetAllAsync(
                filter: ca => ca.ProjectId == projectId && !ca.IsDeleted,
                cancellationToken: cancellationToken);

        // Get latest EVM records for each control account
        var evmRecords = new List<EVMRecord>();
        foreach (var ca in controlAccounts)
        {
            var latestRecord = await _unitOfWork.Repository<EVMRecord>()
                .Query()
                .Where(e => e.ControlAccountId == ca.Id && e.DataDate <= effectiveDate)
                .OrderByDescending(e => e.DataDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestRecord != null)
                evmRecords.Add(latestRecord);
        }

        // Aggregate EVM values
        var totalPV = evmRecords.Sum(e => e.PV);
        var totalEV = evmRecords.Sum(e => e.EV);
        var totalAC = evmRecords.Sum(e => e.AC);
        var totalBAC = evmRecords.Sum(e => e.BAC);
        var totalEAC = evmRecords.Sum(e => e.EAC);
        
        var report = new EVMPerformanceReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ReportDate = effectiveDate,
            TotalPV = totalPV,
            TotalEV = totalEV,
            TotalAC = totalAC,
            TotalBAC = totalBAC,
            TotalCV = totalEV - totalAC,
            TotalSV = totalEV - totalPV,
            OverallCPI = totalAC > 0 ? totalEV / totalAC : 0,
            OverallSPI = totalPV > 0 ? totalEV / totalPV : 0,
            ProjectEAC = totalEAC,
            ProjectVAC = totalBAC - totalEAC,
            ProjectPercentComplete = totalBAC > 0 ? (totalEV / totalBAC) * 100 : 0,

            // Control account breakdowns
            ControlAccounts = evmRecords.Select(e => new ControlAccountEVMDto
            {
                ControlAccountId = e.ControlAccountId,
                Code = e.ControlAccount?.Code ?? string.Empty,
                Name = e.ControlAccount?.Name ?? string.Empty,
                CAMName = e.ControlAccount?.CAMUser?.DisplayName ?? string.Empty,
                PV = e.PV,
                EV = e.EV,
                AC = e.AC,
                BAC = e.BAC,
                EAC = e.EAC,
                CV = e.CV,
                SV = e.SV,
                CPI = e.CPI,
                SPI = e.SPI,
                VAC = e.VAC,
                PercentComplete = e.BAC > 0 ? (e.EV / e.BAC) * 100 : 0,
                Status = e.Status
            }).ToList()
        };

        return report;
    }

    public async Task<List<EVMTrendDto>> GetEVMTrendsAsync(
        Guid controlAccountId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var records = await _unitOfWork.Repository<EVMRecord>()
            .GetAllAsync(
                filter: e => e.ControlAccountId == controlAccountId &&
                           e.DataDate >= startDate &&
                           e.DataDate <= endDate,
                orderBy: q => q.OrderBy(e => e.DataDate),
                cancellationToken: cancellationToken);

        return records.Select(e => new EVMTrendDto
        {
            DataDate = e.DataDate,
            PV = e.PV,
            EV = e.EV,
            AC = e.AC,
            CPI = e.CPI,
            SPI = e.SPI
        }).ToList();
    }

    #endregion

    #region Command Operations

    public async Task<Result<Guid>> CreateEVMRecordAsync(
        CreateEVMRecordDto dto,
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

            // Check if record already exists for this period
            var existingRecord = await _unitOfWork.Repository<EVMRecord>()
                .AnyAsync(
                    e => e.ControlAccountId == dto.ControlAccountId &&
                         e.DataDate.Date == dto.DataDate.Date &&
                         e.PeriodType == dto.PeriodType,
                    cancellationToken);

            if (existingRecord)
                return Result<Guid>.Failure("EVM record already exists for this period");

            // Create EVM record
            var evmRecord = new EVMRecord(
                dto.ControlAccountId,
                dto.DataDate,
                dto.PeriodType,
                dto.PV,
                dto.EV,
                dto.AC,
                dto.BAC);

            evmRecord.CreatedBy = userId;

            // UpdatePercentComplete removed as PlannedPercentComplete and ActualPercentComplete are not in CreateEVMRecordDto

            if (!string.IsNullOrWhiteSpace(dto.Comments))
                evmRecord.AddComments(dto.Comments, userId);

            await _unitOfWork.Repository<EVMRecord>().AddAsync(evmRecord);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(evmRecord.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create EVM record: {ex.Message}");
        }
    }

    public async Task<Result> UpdateEVMActualsAsync(
        Guid id,
        UpdateEVMActualsDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var evmRecord = await _unitOfWork.Repository<EVMRecord>()
                .GetAsync(
                    filter: e => e.Id == id,
                    cancellationToken: cancellationToken);

            if (evmRecord == null)
                return Result.Failure("EVM record not found");

            // Update EV and AC only (PV is not in UpdateEVMActualsDto)
            var currentPV = evmRecord.PV; // Keep current PV
            evmRecord.UpdateValues(currentPV, dto.EV, dto.AC, userId);

            // UpdatePercentComplete removed as PlannedPercentComplete and ActualPercentComplete are not in CreateEVMRecordDto

            if (!string.IsNullOrWhiteSpace(dto.Comments))
                evmRecord.AddComments(dto.Comments, userId);

            _unitOfWork.Repository<EVMRecord>().Update(evmRecord);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update EVM actuals: {ex.Message}");
        }
    }

    public async Task<Result> CalculateProjectEVMAsync(
        Guid projectId,
        DateTime dataDate,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get all control accounts for the project
            var controlAccounts = await _unitOfWork.Repository<ControlAccount>()
                .GetAllAsync(
                    filter: ca => ca.ProjectId == projectId && !ca.IsDeleted,
                    includeProperties: "WorkPackageDetails",
                    cancellationToken: cancellationToken);

            foreach (var controlAccount in controlAccounts)
            {
                // Calculate PV, EV, AC for the control account
                decimal pv = 0, ev = 0, ac = 0;
                decimal bac = controlAccount.TotalBudget;

                // Get all cost items for this control account
                var costItems = await _unitOfWork.Repository<CostItem>()
                    .GetAllAsync(
                        filter: ci => ci.ControlAccountId == controlAccount.Id && !ci.IsDeleted,
                        cancellationToken: cancellationToken);

                // Calculate values
                pv = costItems.Sum(ci => ci.PlannedCost); // Simplified - should consider time-phased budget
                ev = CalculateEarnedValue(controlAccount, costItems, dataDate); // Custom calculation based on progress
                ac = costItems.Sum(ci => ci.ActualCost);

                // Create or update EVM record
                var existingRecord = await _unitOfWork.Repository<EVMRecord>()
                    .GetAsync(
                        filter: e => e.ControlAccountId == controlAccount.Id &&
                                   e.DataDate.Date == dataDate.Date &&
                                   e.PeriodType == EVMPeriodType.Monthly,
                        cancellationToken: cancellationToken);

                if (existingRecord != null)
                {
                    existingRecord.UpdateValues(pv, ev, ac, userId);
                    _unitOfWork.Repository<EVMRecord>().Update(existingRecord);
                }
                else
                {
                    var newRecord = new EVMRecord(
                        controlAccount.Id,
                        dataDate,
                        EVMPeriodType.Monthly,
                        pv,
                        ev,
                        ac,
                        bac);
                    
                    newRecord.CreatedBy = userId;
                    await _unitOfWork.Repository<EVMRecord>().AddAsync(newRecord);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to calculate project EVM: {ex.Message}");
        }
    }

    public async Task<Result> GenerateMonthlyEVMAsync(
        Guid projectId,
        int year,
        int month,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            return await CalculateProjectEVMAsync(projectId, lastDayOfMonth, userId, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to generate monthly EVM: {ex.Message}");
        }
    }

    public async Task<Result> UpdateEACAsync(
        Guid evmRecordId,
        decimal newEAC,
        string justification,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var evmRecord = await _unitOfWork.Repository<EVMRecord>()
                .GetAsync(
                    filter: e => e.Id == evmRecordId,
                    cancellationToken: cancellationToken);

            if (evmRecord == null)
                return Result.Failure("EVM record not found");

            // Store the new EAC value
            // Note: The EVMRecord entity doesn't have a method to update EAC directly
            // We'll add a comment with the justification
            evmRecord.AddComments($"EAC updated to {newEAC:C}. Justification: {justification}", userId);

            _unitOfWork.Repository<EVMRecord>().Update(evmRecord);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update EAC: {ex.Message}");
        }
    }

    #endregion

    #region Nine Column Report Operations

    public async Task<NineColumnReportDto> GetNineColumnReportAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        var effectiveDate = asOfDate ?? DateTime.UtcNow;

        // Get project
        var project = await _unitOfWork.Repository<Project>()
            .GetAsync(
                filter: p => p.Id == projectId,
                includeProperties: "ControlAccounts,WBSElements",
                cancellationToken: cancellationToken);

        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Get all control accounts and their latest EVM data
        var reportLines = new List<NineColumnReportLineDto>();

        foreach (var controlAccount in project.ControlAccounts.Where(ca => !ca.IsDeleted))
        {
            // Get latest EVM record
            var evmRecord = await _unitOfWork.Repository<EVMRecord>()
                .Query()
                .Where(e => e.ControlAccountId == controlAccount.Id && e.DataDate <= effectiveDate)
                .OrderByDescending(e => e.DataDate)
                .FirstOrDefaultAsync(cancellationToken);

            // Get cost items for actual costs
            var costItems = await _unitOfWork.Repository<CostItem>()
                .GetAllAsync(
                    filter: ci => ci.ControlAccountId == controlAccount.Id && !ci.IsDeleted,
                    cancellationToken: cancellationToken);

            var actualCost = costItems.Sum(ci => ci.ActualCost);
            var plannedValue = evmRecord?.PV ?? controlAccount.TotalBudget;
            var earnedValue = evmRecord?.EV ?? 0;
            var physicalProgress = plannedValue > 0 ? (earnedValue / plannedValue) * 100 : 0;
            
            var line = new NineColumnReportLineDto
            {
                Id = controlAccount.Id,
                Code = controlAccount.Code,
                ActivityDescription = controlAccount.Name,
                Level = 2, // Control Account level
                IsControlAccount = true,
                
                // Column 2: Planned Value (Budget)
                PlannedValue = plannedValue,
                
                // Column 3: Physical Progress %
                PhysicalProgressPercentage = physicalProgress,
                
                // Column 4: Earned Value
                EarnedValue = earnedValue,
                
                // Column 5: Actual Cost
                ActualCost = actualCost,
                
                // Column 6: Cost Variance
                CostVariance = earnedValue - actualCost,
                
                // Column 7: Schedule Variance
                ScheduleVariance = earnedValue - plannedValue,
                
                // Column 8: Cost Performance Index
                CostPerformanceIndex = actualCost > 0 ? earnedValue / actualCost : 0,
                
                // Column 9: Estimate at Completion
                EstimateAtCompletion = evmRecord?.EAC ?? controlAccount.TotalBudget
            };

            reportLines.Add(line);
        }

        // Create totals
        var totalPlannedValue = reportLines.Sum(l => l.PlannedValue);
        var totalEarnedValue = reportLines.Sum(l => l.EarnedValue);
        var totalActualCost = reportLines.Sum(l => l.ActualCost);
        
        var totals = new NineColumnReportTotalsDto
        {
            TotalPlannedValue = totalPlannedValue,
            TotalEarnedValue = totalEarnedValue,
            TotalActualCost = totalActualCost,
            TotalCostVariance = reportLines.Sum(l => l.CostVariance),
            TotalScheduleVariance = reportLines.Sum(l => l.ScheduleVariance),
            TotalEstimateAtCompletion = reportLines.Sum(l => l.EstimateAtCompletion),
            OverallPhysicalProgress = totalPlannedValue > 0 ? (totalEarnedValue / totalPlannedValue) * 100 : 0,
            OverallCPI = totalActualCost > 0 ? totalEarnedValue / totalActualCost : 0,
            OverallSPI = totalPlannedValue > 0 ? totalEarnedValue / totalPlannedValue : 0,
            BudgetAtCompletion = project.ControlAccounts.Sum(ca => ca.TotalBudget),
            TotalBudget = project.ControlAccounts.Sum(ca => ca.TotalBudget)
        };

        var report = new NineColumnReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            ReportDate = effectiveDate,
            Currency = project.Currency,
            Lines = reportLines,
            Totals = totals
        };

        return report;
    }

    public async Task<NineColumnReportDto> GetFilteredNineColumnReportAsync(
        NineColumnReportFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var report = await GetNineColumnReportAsync(filter.ProjectId, filter.AsOfDate, cancellationToken);

        // Apply filters
        if (filter.ControlAccountIds != null && filter.ControlAccountIds.Any())
        {
            report.Lines = report.Lines.Where(l => filter.ControlAccountIds.Contains(l.Id)).ToList();
        }

        if (filter.MinLevel.HasValue)
        {
            report.Lines = report.Lines.Where(l => l.Level >= filter.MinLevel.Value).ToList();
        }
        
        if (filter.MaxLevel.HasValue)
        {
            report.Lines = report.Lines.Where(l => l.Level <= filter.MaxLevel.Value).ToList();
        }

        if (filter.ShowOnlyItemsWithVariance)
        {
            report.Lines = report.Lines.Where(l => 
                l.CostVariance != 0 || 
                l.ScheduleVariance != 0).ToList();
        }

        // Recalculate totals
        var totalPlannedValue = report.Lines.Sum(l => l.PlannedValue);
        var totalEarnedValue = report.Lines.Sum(l => l.EarnedValue);
        var totalActualCost = report.Lines.Sum(l => l.ActualCost);
        
        report.Totals = new NineColumnReportTotalsDto
        {
            TotalPlannedValue = totalPlannedValue,
            TotalEarnedValue = totalEarnedValue,
            TotalActualCost = totalActualCost,
            TotalCostVariance = report.Lines.Sum(l => l.CostVariance),
            TotalScheduleVariance = report.Lines.Sum(l => l.ScheduleVariance),
            TotalEstimateAtCompletion = report.Lines.Sum(l => l.EstimateAtCompletion),
            OverallPhysicalProgress = totalPlannedValue > 0 ? (totalEarnedValue / totalPlannedValue) * 100 : 0,
            OverallCPI = totalActualCost > 0 ? totalEarnedValue / totalActualCost : 0,
            OverallSPI = totalPlannedValue > 0 ? totalEarnedValue / totalPlannedValue : 0
        };

        return report;
    }

    public async Task<byte[]> ExportNineColumnReportToExcelAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        // This would require a library like EPPlus or similar for Excel generation
        // For now, returning empty array as placeholder
        await Task.CompletedTask;
        return new byte[0];
    }

    public async Task<byte[]> ExportNineColumnReportToPdfAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        // This would require a library like iTextSharp or similar for PDF generation
        // For now, returning empty array as placeholder
        await Task.CompletedTask;
        return new byte[0];
    }

    public async Task<NineColumnReportDto> GetNineColumnReportByControlAccountAsync(
        Guid controlAccountId,
        DateTime? asOfDate = null,
        bool includeChildren = true,
        CancellationToken cancellationToken = default)
    {
        var effectiveDate = asOfDate ?? DateTime.UtcNow;

        // Get control account
        var controlAccount = await _unitOfWork.Repository<ControlAccount>()
            .GetAsync(
                filter: ca => ca.Id == controlAccountId && !ca.IsDeleted,
                includeProperties: "Project,WorkPackageDetails",
                cancellationToken: cancellationToken);

        if (controlAccount == null)
            throw new InvalidOperationException("Control Account not found");

        var filter = new NineColumnReportFilterDto
        {
            ProjectId = controlAccount.ProjectId,
            AsOfDate = effectiveDate,
            ControlAccountIds = new List<Guid> { controlAccountId }
        };

        return await GetFilteredNineColumnReportAsync(filter, cancellationToken);
    }

    public async Task<List<NineColumnReportDto>> GetNineColumnReportTrendAsync(
        Guid projectId,
        DateTime startDate,
        DateTime endDate,
        string periodType = "Monthly",
        CancellationToken cancellationToken = default)
    {
        var reports = new List<NineColumnReportDto>();
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            var report = await GetNineColumnReportAsync(projectId, currentDate, cancellationToken);
            reports.Add(report);

            // Move to next period
            currentDate = periodType.ToLower() switch
            {
                "daily" => currentDate.AddDays(1),
                "weekly" => currentDate.AddDays(7),
                "monthly" => currentDate.AddMonths(1),
                _ => currentDate.AddMonths(1)
            };
        }

        return reports;
    }

    public async Task<Result<NineColumnReportValidationResult>> ValidateNineColumnReportDataAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        var report = await GetNineColumnReportAsync(projectId, asOfDate, cancellationToken);
        
        var validationResult = new NineColumnReportValidationResult
        {
            IsValid = true,
            Errors = new List<string>(),
            Warnings = new List<string>()
        };

        // Validation rules
        foreach (var line in report.Lines)
        {
            // Check if cost overrun
            if (line.CostVariance < 0)
            {
                validationResult.Warnings.Add($"{line.Code}: Cost overrun detected (CV: {line.CostVariance:C})");
            }

            // Check if behind schedule
            if (line.ScheduleVariance < 0)
            {
                validationResult.Warnings.Add($"{line.Code}: Behind schedule (SV: {line.ScheduleVariance:C})");
            }

            // Check CPI threshold
            if (line.CostPerformanceIndex < 0.9m && line.CostPerformanceIndex > 0)
            {
                validationResult.Errors.Add($"{line.Code}: Critical cost performance (CPI: {line.CostPerformanceIndex:F2})");
                validationResult.IsValid = false;
            }

            // Check earned value consistency
            if (line.EarnedValue > line.PlannedValue)
            {
                validationResult.Warnings.Add($"{line.Code}: Earned Value exceeds Planned Value");
            }
        }

        return Result<NineColumnReportValidationResult>.Success(validationResult);
    }

    #endregion

    #region Private Methods

    private decimal CalculateEarnedValue(ControlAccount controlAccount, IEnumerable<CostItem> costItems, DateTime dataDate)
    {
        // Simplified earned value calculation
        // In a real implementation, this would consider:
        // - Work package progress
        // - Measurement methods (% complete, milestones, etc.)
        // - Time-phased budgets
        
        // For now, using actual cost as a proxy for progress
        var totalActualCost = costItems.Sum(ci => ci.ActualCost);
        var totalPlannedCost = costItems.Sum(ci => ci.PlannedCost);
        
        if (totalPlannedCost == 0)
            return 0;
            
        var progressPercentage = Math.Min(totalActualCost / totalPlannedCost, 1.0m);
        return controlAccount.TotalBudget * progressPercentage;
    }

    #endregion
}