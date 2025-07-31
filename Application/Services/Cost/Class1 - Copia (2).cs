using Core.DTOs.EVM;
using Core.Enums.Cost;
using Domain.Entities.Cost;
using Domain.Entities.EVM;
using Domain.Entities.Projects;


namespace Application.Services.Cost;

/// <summary>
/// Implementation of Earned Value Management service operations
/// </summary>
public class EVMService : IEVMService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogger<EVMService> _logger;

    public EVMService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserContext currentUser,
        ILogger<EVMService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    #region Query Operations

    public async Task<PagedResult<EVMRecordDto>> GetEVMRecordsAsync(
        QueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _unitOfWork.Repository<EVMRecord>()
                .Query()
                .Include(e => e.ControlAccount)
                    .ThenInclude(ca => ca.Project);

            // Apply filters
            if (parameters.Filters.TryGetValue("projectId", out var projectId))
            {
                if (Guid.TryParse(projectId, out var projectGuid))
                {
                    query = query.Where(e => e.ControlAccount.ProjectId == projectGuid);
                }
            }

            if (parameters.Filters.TryGetValue("controlAccountId", out var controlAccountId))
            {
                if (Guid.TryParse(controlAccountId, out var caGuid))
                {
                    query = query.Where(e => e.ControlAccountId == caGuid);
                }
            }

            if (parameters.Filters.TryGetValue("reportDate", out var reportDate))
            {
                if (DateTime.TryParse(reportDate, out var date))
                {
                    query = query.Where(e => e.ReportDate.Date == date.Date);
                }
            }

            if (parameters.Filters.TryGetValue("status", out var status))
            {
                if (Enum.TryParse<EVMStatus>(status, out var statusEnum))
                {
                    query = query.Where(e => e.Status == statusEnum);
                }
            }

            if (parameters.Filters.TryGetValue("isApproved", out var isApproved))
            {
                if (bool.TryParse(isApproved, out var isApprovedBool))
                {
                    query = query.Where(e => e.IsApproved == isApprovedBool);
                }
            }

            // Apply sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "reportdate" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(e => e.ReportDate)
                    : query.OrderBy(e => e.ReportDate),
                "controlaccount" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(e => e.ControlAccount.Code)
                    : query.OrderBy(e => e.ControlAccount.Code),
                "cpi" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(e => e.CPI)
                    : query.OrderBy(e => e.CPI),
                "spi" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(e => e.SPI)
                    : query.OrderBy(e => e.SPI),
                "status" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(e => e.Status)
                    : query.OrderBy(e => e.Status),
                _ => query.OrderByDescending(e => e.ReportDate)
                    .ThenBy(e => e.ControlAccount.Code)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<EVMRecordDto>>(items);

            return new PagedResult<EVMRecordDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing health check for project: {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<List<EVMAlertDto>> GetEVMAlertsAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var alerts = await _unitOfWork.Repository<EVMAlert>()
                .Query()
                .Include(a => a.ControlAccount)
                .Where(a => a.ProjectId == projectId && !a.IsAcknowledged)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<EVMAlertDto>>(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting EVM alerts for project: {ProjectId}", projectId);
            throw;
        }
    }

    #endregion

    #region Private Methods

    private async Task<(decimal pv, decimal ev, decimal ac)> CalculateControlAccountMetrics(
        ControlAccount controlAccount,
        DateTime reportDate,
        CancellationToken cancellationToken)
    {
        decimal plannedValue = 0;
        decimal earnedValue = 0;
        decimal actualCost = 0;

        foreach (var workPackage in controlAccount.WorkPackages.Where(wp => !wp.IsDeleted))
        {
            // Calculate Planned Value (PV)
            if (workPackage.PlannedStartDate <= reportDate)
            {
                if (workPackage.PlannedEndDate <= reportDate)
                {
                    // Work package should be complete by report date
                    plannedValue += workPackage.Budget;
                }
                else
                {
                    // Work package is in progress
                    var totalDuration = (workPackage.PlannedEndDate - workPackage.PlannedStartDate).TotalDays;
                    var elapsedDuration = (reportDate - workPackage.PlannedStartDate).TotalDays;
                    var plannedProgress = totalDuration > 0 ? elapsedDuration / totalDuration : 0;
                    plannedValue += workPackage.Budget * (decimal)plannedProgress;
                }
            }

            // Calculate Earned Value (EV)
            earnedValue += workPackage.Budget * (workPackage.PercentComplete / 100);

            // Calculate Actual Cost (AC)
            actualCost += workPackage.ActualCost;
        }

        return (plannedValue, earnedValue, actualCost);
    }

    private async Task GenerateEVMAlertsAsync(
        Guid projectId,
        DateTime reportDate,
        string userId,
        CancellationToken cancellationToken)
    {
        var evmReport = await GetProjectEVMReportAsync(projectId, cancellationToken);

        // Generate project-level alerts
        if (evmReport.CPI < 0.90m)
        {
            var alert = new EVMAlert(
                projectId,
                null,
                "CostOverrun",
                "Critical",
                $"Project CPI is {evmReport.CPI:F2} - Significant cost overrun detected");
            alert.CreatedBy = userId;
            await _unitOfWork.Repository<EVMAlert>().AddAsync(alert, cancellationToken);
        }

        if (evmReport.SPI < 0.90m)
        {
            var alert = new EVMAlert(
                projectId,
                null,
                "ScheduleDelay",
                "Critical",
                $"Project SPI is {evmReport.SPI:F2} - Significant schedule delay detected");
            alert.CreatedBy = userId;
            await _unitOfWork.Repository<EVMAlert>().AddAsync(alert, cancellationToken);
        }

        // Generate control account-level alerts
        foreach (var caDetail in evmReport.ControlAccountDetails)
        {
            if (caDetail.CPI < 0.85m)
            {
                var alert = new EVMAlert(
                    projectId,
                    caDetail.ControlAccountId,
                    "CostOverrun",
                    "High",
                    $"Control Account {caDetail.ControlAccountCode} CPI is {caDetail.CPI:F2}");
                alert.CreatedBy = userId;
                await _unitOfWork.Repository<EVMAlert>().AddAsync(alert, cancellationToken);
            }

            if (caDetail.SPI < 0.85m)
            {
                var alert = new EVMAlert(
                    projectId,
                    caDetail.ControlAccountId,
                    "ScheduleDelay",
                    "High",
                    $"Control Account {caDetail.ControlAccountCode} SPI is {caDetail.SPI:F2}");
                alert.CreatedBy = userId;
                await _unitOfWork.Repository<EVMAlert>().AddAsync(alert, cancellationToken);
            }
        }
    }

    private decimal CalculateOptimisticEAC(EVMPerformanceReportDto report)
    {
        // Optimistic: Assume performance will improve to planned levels
        return report.ActualCost + (report.BudgetAtCompletion - report.EarnedValue);
    }

    private decimal CalculatePessimisticEAC(EVMPerformanceReportDto report)
    {
        // Pessimistic: Assume both cost and schedule performance affect remaining work
        var compositeCPI = report.CPI * report.SPI;
        return compositeCPI > 0 
            ? report.ActualCost + ((report.BudgetAtCompletion - report.EarnedValue) / compositeCPI)
            : report.BudgetAtCompletion * 1.5m; // 50% overrun as worst case
    }

    private decimal CalculateRiskAdjustedEAC(EVMPerformanceReportDto report)
    {
        // Use weighted average of optimistic, most likely, and pessimistic
        var optimistic = CalculateOptimisticEAC(report);
        var mostLikely = report.EAC;
        var pessimistic = CalculatePessimisticEAC(report);

        // PERT formula: (O + 4ML + P) / 6
        return (optimistic + (4 * mostLikely) + pessimistic) / 6;
    }

    private DateTime CalculateEstimatedCompletionDate(EVMPerformanceReportDto report)
    {
        // Simple calculation based on SPI
        // This would need project schedule information for accurate calculation
        var remainingDuration = 180; // Default 6 months
        if (report.SPI > 0)
        {
            remainingDuration = (int)(remainingDuration / report.SPI);
        }
        return DateTime.UtcNow.AddDays(remainingDuration);
    }

    private decimal CalculateConfidenceLevel(EVMPerformanceReportDto report)
    {
        // Calculate confidence based on performance stability
        var baseConfidence = 50m;
        
        // Adjust based on CPI
        if (report.CPI >= 0.95m && report.CPI <= 1.05m)
            baseConfidence += 20;
        else if (report.CPI >= 0.90m && report.CPI <= 1.10m)
            baseConfidence += 10;
        else
            baseConfidence -= 10;

        // Adjust based on SPI
        if (report.SPI >= 0.95m && report.SPI <= 1.05m)
            baseConfidence += 20;
        else if (report.SPI >= 0.90m && report.SPI <= 1.10m)
            baseConfidence += 10;
        else
            baseConfidence -= 10;

        // Adjust based on percent complete
        if (report.PercentComplete > 75)
            baseConfidence += 10;
        else if (report.PercentComplete < 25)
            baseConfidence -= 10;

        return Math.Max(0, Math.Min(100, baseConfidence));
    }

    private List<string> GenerateRecommendations(EVMPerformanceReportDto report)
    {
        var recommendations = new List<string>();

        if (report.CPI < 0.95m)
        {
            recommendations.Add("Review and reduce project costs where possible");
            recommendations.Add("Negotiate better rates with vendors and subcontractors");
            recommendations.Add("Improve cost control processes");
        }

        if (report.SPI < 0.95m)
        {
            recommendations.Add("Fast-track critical path activities");
            recommendations.Add("Consider adding resources to delayed activities");
            recommendations.Add("Review and optimize project schedule");
        }

        if (report.TCPI > 1.10m)
        {
            recommendations.Add("Performance improvement required to meet budget targets");
            recommendations.Add("Consider requesting additional budget or reducing scope");
        }

        if (report.VAC < 0)
        {
            recommendations.Add("Project is forecasted to exceed budget");
            recommendations.Add("Implement cost reduction measures immediately");
        }

        return recommendations;
    }

    private string DetermineOverallHealth(EVMPerformanceReportDto report)
    {
        var criticalCount = 0;
        var warningCount = 0;

        // Check CPI
        if (report.CPI < 0.90m) criticalCount++;
        else if (report.CPI < 0.95m) warningCount++;

        // Check SPI
        if (report.SPI < 0.90m) criticalCount++;
        else if (report.SPI < 0.95m) warningCount++;

        // Check VAC
        if (report.VAC < -report.BudgetAtCompletion * 0.10m) criticalCount++;
        else if (report.VAC < 0) warningCount++;

        // Determine overall health
        if (criticalCount > 0)
            return "Critical";
        else if (warningCount > 1)
            return "Warning";
        else
            return "Good";
    }

    #endregion
}ex, "Error getting EVM records");
            throw;
        }
    }

    public async Task<EVMRecordDetailDto?> GetEVMRecordByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var evmRecord = await _unitOfWork.Repository<EVMRecord>()
                .Query()
                .Include(e => e.ControlAccount)
                    .ThenInclude(ca => ca.Project)
                .Include(e => e.ControlAccount)
                    .ThenInclude(ca => ca.CAM)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (evmRecord == null)
                return null;

            return _mapper.Map<EVMRecordDetailDto>(evmRecord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting EVM record by id: {Id}", id);
            throw;
        }
    }

    public async Task<EVMPerformanceReportDto> GetProjectEVMReportAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(projectId, cancellationToken);

            if (project == null)
                throw new InvalidOperationException("Project not found");

            // Get latest EVM records for each control account
            var controlAccounts = await _unitOfWork.Repository<ControlAccount>()
                .Query()
                .Where(ca => ca.ProjectId == projectId && !ca.IsDeleted && ca.IsActive)
                .ToListAsync(cancellationToken);

            var latestEVMRecords = new List<EVMRecord>();
            foreach (var ca in controlAccounts)
            {
                var latestRecord = await _unitOfWork.Repository<EVMRecord>()
                    .Query()
                    .Where(e => e.ControlAccountId == ca.Id)
                    .OrderByDescending(e => e.ReportDate)
                    .FirstOrDefaultAsync(cancellationToken);

                if (latestRecord != null)
                    latestEVMRecords.Add(latestRecord);
            }

            // Calculate project-level metrics
            var totalPV = latestEVMRecords.Sum(e => e.PlannedValue);
            var totalEV = latestEVMRecords.Sum(e => e.EarnedValue);
            var totalAC = latestEVMRecords.Sum(e => e.ActualCost);
            var totalBAC = latestEVMRecords.Sum(e => e.BudgetAtCompletion);

            var report = new EVMPerformanceReportDto
            {
                ProjectId = projectId,
                ProjectName = project.Name,
                ReportDate = DateTime.UtcNow,
                PlannedValue = totalPV,
                EarnedValue = totalEV,
                ActualCost = totalAC,
                BudgetAtCompletion = totalBAC,
                CostVariance = totalEV - totalAC,
                ScheduleVariance = totalEV - totalPV,
                CPI = totalAC > 0 ? totalEV / totalAC : 0,
                SPI = totalPV > 0 ? totalEV / totalPV : 0,
                EAC = totalBAC > 0 && totalEV > 0 && totalAC > 0 
                    ? totalBAC / (totalEV / totalAC) 
                    : totalBAC,
                ETC = 0, // Will be calculated below
                VAC = 0, // Will be calculated below
                TCPI = 0, // Will be calculated below
                PercentComplete = totalBAC > 0 ? (totalEV / totalBAC) * 100 : 0,
                PercentSpent = totalBAC > 0 ? (totalAC / totalBAC) * 100 : 0,
                ControlAccountDetails = new List<EVMControlAccountDetailDto>()
            };

            // Calculate remaining metrics
            report.ETC = report.EAC - totalAC;
            report.VAC = totalBAC - report.EAC;
            
            var remainingWork = totalBAC - totalEV;
            var remainingBudget = totalBAC - totalAC;
            report.TCPI = remainingBudget > 0 ? remainingWork / remainingBudget : 0;

            // Determine overall status
            if (report.CPI >= 0.95m && report.CPI <= 1.05m && report.SPI >= 0.95m && report.SPI <= 1.05m)
            {
                report.OverallStatus = "Good";
            }
            else if (report.CPI >= 0.90m && report.CPI <= 1.10m && report.SPI >= 0.90m && report.SPI <= 1.10m)
            {
                report.OverallStatus = "Warning";
            }
            else
            {
                report.OverallStatus = "Critical";
            }

            // Add control account details
            foreach (var evmRecord in latestEVMRecords)
            {
                var caDetail = new EVMControlAccountDetailDto
                {
                    ControlAccountId = evmRecord.ControlAccountId,
                    ControlAccountCode = evmRecord.ControlAccount.Code,
                    ControlAccountName = evmRecord.ControlAccount.Name,
                    PlannedValue = evmRecord.PlannedValue,
                    EarnedValue = evmRecord.EarnedValue,
                    ActualCost = evmRecord.ActualCost,
                    CPI = evmRecord.CPI,
                    SPI = evmRecord.SPI,
                    Status = evmRecord.Status.ToString()
                };
                report.ControlAccountDetails.Add(caDetail);
            }

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating project EVM report for project: {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<List<EVMTrendDto>> GetEVMTrendsAsync(
        Guid controlAccountId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var evmRecords = await _unitOfWork.Repository<EVMRecord>()
                .Query()
                .Where(e => e.ControlAccountId == controlAccountId)
                .OrderBy(e => e.ReportDate)
                .ToListAsync(cancellationToken);

            var trends = evmRecords.Select(e => new EVMTrendDto
            {
                ReportDate = e.ReportDate,
                PlannedValue = e.PlannedValue,
                EarnedValue = e.EarnedValue,
                ActualCost = e.ActualCost,
                CostVariance = e.CostVariance,
                ScheduleVariance = e.ScheduleVariance,
                CPI = e.CPI,
                SPI = e.SPI,
                EAC = e.EAC,
                PercentComplete = e.PercentComplete
            }).ToList();

            return trends;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting EVM trends for control account: {ControlAccountId}", controlAccountId);
            throw;
        }
    }

    public async Task<EVMForecastDto> GetProjectForecastAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var evmReport = await GetProjectEVMReportAsync(projectId, cancellationToken);

            var forecast = new EVMForecastDto
            {
                ProjectId = projectId,
                ForecastDate = DateTime.UtcNow,
                CurrentEAC = evmReport.EAC,
                OptimisticEAC = CalculateOptimisticEAC(evmReport),
                MostLikelyEAC = evmReport.EAC,
                PessimisticEAC = CalculatePessimisticEAC(evmReport),
                EstimatedCompletionDate = CalculateEstimatedCompletionDate(evmReport),
                RiskAdjustedEAC = CalculateRiskAdjustedEAC(evmReport),
                ConfidenceLevel = CalculateConfidenceLevel(evmReport),
                Recommendations = GenerateRecommendations(evmReport)
            };

            return forecast;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating project forecast for project: {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<List<EVMComparisonDto>> ComparePeriodsAsync(
        Guid projectId,
        DateTime period1,
        DateTime period2,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get control accounts for the project
            var controlAccounts = await _unitOfWork.Repository<ControlAccount>()
                .Query()
                .Where(ca => ca.ProjectId == projectId && !ca.IsDeleted)
                .ToListAsync(cancellationToken);

            var comparisons = new List<EVMComparisonDto>();

            foreach (var ca in controlAccounts)
            {
                // Get EVM record for period 1
                var record1 = await _unitOfWork.Repository<EVMRecord>()
                    .Query()
                    .Where(e => e.ControlAccountId == ca.Id && 
                               e.ReportDate.Date == period1.Date)
                    .FirstOrDefaultAsync(cancellationToken);

                // Get EVM record for period 2
                var record2 = await _unitOfWork.Repository<EVMRecord>()
                    .Query()
                    .Where(e => e.ControlAccountId == ca.Id && 
                               e.ReportDate.Date == period2.Date)
                    .FirstOrDefaultAsync(cancellationToken);

                if (record1 != null && record2 != null)
                {
                    var comparison = new EVMComparisonDto
                    {
                        ControlAccountId = ca.Id,
                        ControlAccountCode = ca.Code,
                        ControlAccountName = ca.Name,
                        Period1Date = period1,
                        Period2Date = period2,
                        PV1 = record1.PlannedValue,
                        PV2 = record2.PlannedValue,
                        PVChange = record2.PlannedValue - record1.PlannedValue,
                        EV1 = record1.EarnedValue,
                        EV2 = record2.EarnedValue,
                        EVChange = record2.EarnedValue - record1.EarnedValue,
                        AC1 = record1.ActualCost,
                        AC2 = record2.ActualCost,
                        ACChange = record2.ActualCost - record1.ActualCost,
                        CPI1 = record1.CPI,
                        CPI2 = record2.CPI,
                        CPIChange = record2.CPI - record1.CPI,
                        SPI1 = record1.SPI,
                        SPI2 = record2.SPI,
                        SPIChange = record2.SPI - record1.SPI
                    };

                    comparisons.Add(comparison);
                }
            }

            return comparisons.OrderBy(c => c.ControlAccountCode).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing EVM periods for project: {ProjectId}", projectId);
            throw;
        }
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
            _logger.LogInformation("Creating EVM record for control account: {ControlAccountId} by user: {UserId}", 
                dto.ControlAccountId, userId);

            // Validate control account exists
            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .GetByIdAsync(dto.ControlAccountId, cancellationToken);
            if (controlAccount == null)
                return Result<Guid>.Failure("Control Account not found");

            // Check if record already exists for this period
            var existingRecord = await _unitOfWork.Repository<EVMRecord>()
                .AnyAsync(e => e.ControlAccountId == dto.ControlAccountId &&
                             e.ReportDate.Date == dto.ReportDate.Date, cancellationToken);

            if (existingRecord)
                return Result<Guid>.Failure($"EVM record already exists for this control account on {dto.ReportDate:yyyy-MM-dd}");

            // Create EVM record
            var evmRecord = new EVMRecord(
                dto.ControlAccountId,
                dto.ReportDate,
                dto.PlannedValue,
                dto.EarnedValue,
                dto.ActualCost,
                dto.BudgetAtCompletion);

            if (!string.IsNullOrWhiteSpace(dto.Comments))
                evmRecord.UpdateComments(dto.Comments);

            evmRecord.CreatedBy = userId;

            await _unitOfWork.Repository<EVMRecord>().AddAsync(evmRecord, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("EVM record created successfully with Id: {Id}", evmRecord.Id);

            return Result<Guid>.Success(evmRecord.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating EVM record");
            return Result<Guid>.Failure($"Error creating EVM record: {ex.Message}");
        }
    }

    public async Task<r> CalculateProjectEVMAsync(
        Guid projectId,
        CalculateEVMDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calculating EVM for project: {ProjectId} on {ReportDate} by user: {UserId}", 
                projectId, dto.ReportDate, userId);

            // Get all active control accounts for the project
            var controlAccounts = await _unitOfWork.Repository<ControlAccount>()
                .Query()
                .Include(ca => ca.WorkPackages)
                    .ThenInclude(wp => wp.ProgressHistory)
                .Where(ca => ca.ProjectId == projectId && 
                           !ca.IsDeleted && 
                           ca.IsActive &&
                           ca.Status != ControlAccountStatus.Planning)
                .ToListAsync(cancellationToken);

            foreach (var ca in controlAccounts)
            {
                // Check if EVM record already exists for this date
                var existingRecord = await _unitOfWork.Repository<EVMRecord>()
                    .AnyAsync(e => e.ControlAccountId == ca.Id &&
                                 e.ReportDate.Date == dto.ReportDate.Date, cancellationToken);

                if (existingRecord)
                    continue; // Skip if already calculated

                // Calculate PV, EV, AC for the control account
                var (pv, ev, ac) = await CalculateControlAccountMetrics(ca, dto.ReportDate, cancellationToken);

                // Create EVM record
                var evmRecord = new EVMRecord(
                    ca.Id,
                    dto.ReportDate,
                    pv,
                    ev,
                    ac,
                    ca.BAC);

                evmRecord.CreatedBy = userId;

                await _unitOfWork.Repository<EVMRecord>().AddAsync(evmRecord, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate alerts if requested
            if (dto.GenerateAlerts)
            {
                await GenerateEVMAlertsAsync(projectId, dto.ReportDate, userId, cancellationToken);
            }

            _logger.LogInformation("EVM calculation completed for project: {ProjectId}", projectId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating project EVM for project: {ProjectId}", projectId);
            return Result.Failure($"Error calculating project EVM: {ex.Message}");
        }
    }

    public async Task<r> RecalculateControlAccountEVMAsync(
        Guid controlAccountId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Recalculating EVM for control account: {ControlAccountId} by user: {UserId}", 
                controlAccountId, userId);

            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .Query()
                .Include(ca => ca.WorkPackages)
                    .ThenInclude(wp => wp.ProgressHistory)
                .FirstOrDefaultAsync(ca => ca.Id == controlAccountId && !ca.IsDeleted, cancellationToken);

            if (controlAccount == null)
                return Result.Failure("Control Account not found");

            // Get all EVM records for this control account
            var evmRecords = await _unitOfWork.Repository<EVMRecord>()
                .Query()
                .Where(e => e.ControlAccountId == controlAccountId && !e.IsApproved)
                .OrderBy(e => e.ReportDate)
                .ToListAsync(cancellationToken);

            foreach (var evmRecord in evmRecords)
            {
                // Recalculate metrics
                var (pv, ev, ac) = await CalculateControlAccountMetrics(
                    controlAccount, 
                    evmRecord.ReportDate, 
                    cancellationToken);

                evmRecord.UpdateValues(pv, ev, ac);
                evmRecord.UpdatedBy = userId;

                _unitOfWork.Repository<EVMRecord>().Update(evmRecord);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("EVM recalculation completed for control account: {ControlAccountId}", controlAccountId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recalculating control account EVM: {ControlAccountId}", controlAccountId);
            return Result.Failure($"Error recalculating control account EVM: {ex.Message}");
        }
    }

    public async Task<r> UpdateEVMRecordAsync(
        Guid id,
        UpdateEVMRecordDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating EVM record: {Id} by user: {UserId}", id, userId);

            var evmRecord = await _unitOfWork.Repository<EVMRecord>()
                .GetByIdAsync(id, cancellationToken);

            if (evmRecord == null)
                return Result.Failure("EVM Record not found");

            if (evmRecord.IsApproved)
                return Result.Failure("Cannot update an approved EVM record");

            evmRecord.UpdateValues(dto.PlannedValue, dto.EarnedValue, dto.ActualCost);
            
            if (!string.IsNullOrWhiteSpace(dto.Comments))
                evmRecord.UpdateComments(dto.Comments);

            evmRecord.UpdatedBy = userId;

            _unitOfWork.Repository<EVMRecord>().Update(evmRecord);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("EVM record updated successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating EVM record: {Id}", id);
            return Result.Failure($"Error updating EVM record: {ex.Message}");
        }
    }

    public async Task<r> ApproveEVMRecordAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving EVM record: {Id} by user: {UserId}", id, userId);

            var evmRecord = await _unitOfWork.Repository<EVMRecord>()
                .GetByIdAsync(id, cancellationToken);

            if (evmRecord == null)
                return Result.Failure("EVM Record not found");

            if (evmRecord.IsApproved)
                return Result.Failure("EVM record is already approved");

            evmRecord.Approve(userId);

            _unitOfWork.Repository<EVMRecord>().Update(evmRecord);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("EVM record approved successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving EVM record: {Id}", id);
            return Result.Failure($"Error approving EVM record: {ex.Message}");
        }
    }

    public async Task<r> DeleteEVMRecordAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting EVM record: {Id} by user: {UserId}", id, userId);

            var evmRecord = await _unitOfWork.Repository<EVMRecord>()
                .GetByIdAsync(id, cancellationToken);

            if (evmRecord == null)
                return Result.Failure("EVM Record not found");

            if (evmRecord.IsApproved)
                return Result.Failure("Cannot delete an approved EVM record");

            _unitOfWork.Repository<EVMRecord>().Remove(evmRecord);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("EVM record deleted successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting EVM record: {Id}", id);
            return Result.Failure($"Error deleting EVM record: {ex.Message}");
        }
    }

    #endregion

    #region Analysis Operations

    public async Task<EVMHealthCheckDto> PerformHealthCheckAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var evmReport = await GetProjectEVMReportAsync(projectId, cancellationToken);

            var healthCheck = new EVMHealthCheckDto
            {
                ProjectId = projectId,
                CheckDate = DateTime.UtcNow,
                OverallHealth = DetermineOverallHealth(evmReport),
                Issues = new List<EVMHealthIssueDto>(),
                Recommendations = new List<EVMHealthRecommendationDto>()
            };

            // Check for CPI issues
            if (evmReport.CPI < 0.95m)
            {
                healthCheck.Issues.Add(new EVMHealthIssueDto
                {
                    Severity = evmReport.CPI < 0.90m ? "Critical" : "High",
                    Category = "Cost Performance",
                    Description = $"Cost Performance Index is {evmReport.CPI:F2}, indicating cost overrun",
                    AffectedArea = "Project Budget"
                });
            }

            // Check for SPI issues
            if (evmReport.SPI < 0.95m)
            {
                healthCheck.Issues.Add(new EVMHealthIssueDto
                {
                    Severity = evmReport.SPI < 0.90m ? "Critical" : "High",
                    Category = "Schedule Performance",
                    Description = $"Schedule Performance Index is {evmReport.SPI:F2}, indicating schedule delay",
                    AffectedArea = "Project Schedule"
                });
            }

            // Check for EAC variance
            var eacVariance = (evmReport.EAC - evmReport.BudgetAtCompletion) / evmReport.BudgetAtCompletion;
            if (eacVariance > 0.10m)
            {
                healthCheck.Issues.Add(new EVMHealthIssueDto
                {
                    Severity = eacVariance > 0.20m ? "Critical" : "High",
                    Category = "Forecast",
                    Description = $"Estimate at Completion exceeds budget by {eacVariance:P}",
                    AffectedArea = "Project Forecast"
                });
            }
                
            // Generate recommendations based on issues
            if (evmReport.OverallCPI < 0.95m)
            {
                healthCheck.Recommendations.Add(new EVMHealthRecommendationDto
                {
                    Priority = "High",
                    Action = "Review and optimize project costs",
                    ExpectedBenefit = "Improve cost performance and reduce overrun"
                });
            }

            if (evmReport.OverallSPI < 0.95m)
            {
                healthCheck.Recommendations.Add(new EVMHealthRecommendationDto
                {
                    Priority = "High",
                    Action = "Accelerate critical path activities",
                    ExpectedBenefit = "Recover schedule delays and improve delivery timeline"
                });
            }

            return healthCheck;
        }
        catch (Exception ex)
        {
            _logger.LogError(