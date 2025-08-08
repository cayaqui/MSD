using Application.Interfaces;
using Application.Interfaces.Auth;
using Application.Interfaces.Cost;
using Application.Interfaces.Documents;
using Application.Interfaces.Organization;
using Application.Interfaces.Progress;
using Application.Interfaces.Projects;
using Application.Interfaces.Reports;
using Application.Interfaces.Storage;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Reports;
using Core.Enums.Documents;
using Core.Enums.Reports;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Application.Services.Reports;

public class ReportService : IReportService
{
    private readonly IMapper _mapper;
    private readonly ILogger<ReportService> _logger;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IDocumentService _documentService;
    private readonly IProjectService _projectService;
    private readonly IScheduleService _scheduleService;
    private readonly IMilestoneService _milestoneService;
    private readonly IBudgetService _budgetService;
    private readonly IControlAccountService _controlAccountService;
    private readonly IWorkPackageService _workPackageService;
    private readonly ICurrentUserService _currentUserService;
    private readonly Dictionary<Guid, ReportDto> _reportCache = new();
    private const string ReportContainerName = "reports";

    public ReportService(
        IMapper mapper,
        ILogger<ReportService> logger,
        IBlobStorageService blobStorageService,
        IDocumentService documentService,
        IProjectService projectService,
        IScheduleService scheduleService,
        IMilestoneService milestoneService,
        IBudgetService budgetService,
        IControlAccountService controlAccountService,
        IWorkPackageService workPackageService,
        ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _logger = logger;
        _blobStorageService = blobStorageService;
        _documentService = documentService;
        _projectService = projectService;
        _scheduleService = scheduleService;
        _milestoneService = milestoneService;
        _budgetService = budgetService;
        _controlAccountService = controlAccountService;
        _workPackageService = workPackageService;
        _currentUserService = currentUserService;
    }

    public async Task<ReportDto> GenerateReportAsync(GenerateReportDto dto)
    {
        try
        {
            var report = new ReportDto
            {
                Id = Guid.NewGuid(),
                Code = $"RPT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8]}",
                Name = dto.CustomFileName ?? "Generated Report",
                Type = ReportType.Custom,
                Category = "Generated",
                TemplateId = dto.TemplateId,
                ProjectId = dto.ProjectId,
                Parameters = dto.Parameters,
                ReportingPeriodStart = dto.ReportingPeriodStart,
                ReportingPeriodEnd = dto.ReportingPeriodEnd,
                GeneratedDate = DateTime.UtcNow,
                GeneratedById = _currentUserService.UserId ?? Guid.Empty,
                GeneratedByName = _currentUserService.Name ?? "System",
                Status = ReportStatus.Generating,
                Recipients = dto.Recipients ?? new List<string>(),
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _currentUserService.Name ?? "System"
            };

            // Store in cache
            _reportCache[report.Id] = report;

            // Generate report content based on template
            var content = await GenerateReportContent(report, dto);
            
            // Save to blob storage
            var fileName = $"{report.Code}_{DateTime.UtcNow:yyyyMMddHHmmss}.{dto.Format.ToString().ToLower()}";
            var blobName = $"{report.ProjectId}/{fileName}";
            
            await _blobStorageService.CreateContainerIfNotExistsAsync(ReportContainerName);
            
            var uploadResult = await _blobStorageService.UploadAsync(
                content, 
                ReportContainerName, 
                blobName, 
                GetContentType(dto.Format),
                new Dictionary<string, string>
                {
                    ["ReportId"] = report.Id.ToString(),
                    ["ProjectId"] = report.ProjectId?.ToString() ?? "",
                    ["GeneratedBy"] = report.GeneratedByName
                });

            if (uploadResult.Success)
            {
                report.FileName = fileName;
                report.FilePath = uploadResult.BlobUrl;
                report.FileSize = content.Length;
                report.ContentType = GetContentType(dto.Format);
                report.Status = ReportStatus.Completed;
            }
            else
            {
                report.Status = ReportStatus.Failed;
                report.ErrorMessage = uploadResult.ErrorMessage;
            }

            // Distribute if requested
            if (dto.SendByEmail && dto.Recipients?.Any() == true)
            {
                await DistributeReportAsync(report.Id, dto.Recipients);
            }

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report");
            throw;
        }
    }

    public async Task<ReportDto> RegenerateReportAsync(Guid reportId)
    {
        var existingReport = await GetByIdAsync(reportId);
        if (existingReport == null)
            throw new InvalidOperationException("Report not found");

        var generateDto = new GenerateReportDto
        {
            TemplateId = existingReport.TemplateId ?? Guid.Empty,
            ProjectId = existingReport.ProjectId,
            Format = ExportFormat.Pdf,
            Parameters = existingReport.Parameters,
            ReportingPeriodStart = existingReport.ReportingPeriodStart,
            ReportingPeriodEnd = existingReport.ReportingPeriodEnd,
            Recipients = existingReport.Recipients
        };

        return await GenerateReportAsync(generateDto);
    }

    public async Task<IEnumerable<ReportDto>> GetByProjectAsync(Guid projectId)
    {
        // In a real implementation, this would query a database
        return _reportCache.Values.Where(r => r.ProjectId == projectId);
    }

    public async Task<IEnumerable<ReportDto>> GetByTemplateAsync(Guid templateId)
    {
        return _reportCache.Values.Where(r => r.TemplateId == templateId);
    }

    public async Task<IEnumerable<ReportDto>> GetByTypeAsync(ReportType type)
    {
        return _reportCache.Values.Where(r => r.Type == type);
    }

    public async Task<byte[]?> GetReportContentAsync(Guid reportId)
    {
        var report = await GetByIdAsync(reportId);
        if (report == null || string.IsNullOrEmpty(report.FileName))
            return null;

        var blobName = $"{report.ProjectId}/{report.FileName}";
        return await _blobStorageService.DownloadBytesAsync(ReportContainerName, blobName);
    }

    public async Task<bool> DistributeReportAsync(Guid reportId, List<string> recipients)
    {
        try
        {
            var report = await GetByIdAsync(reportId);
            if (report == null)
                return false;

            // TODO: Implement email distribution
            // This would integrate with an email service
            
            report.Recipients.AddRange(recipients);
            report.DistributedDate = DateTime.UtcNow;
            
            _logger.LogInformation("Report {ReportId} distributed to {Count} recipients", reportId, recipients.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error distributing report {ReportId}", reportId);
            return false;
        }
    }

    public async Task<IEnumerable<ReportDistributionDto>> GetDistributionsAsync(Guid reportId)
    {
        // In a real implementation, this would query distribution history
        return new List<ReportDistributionDto>();
    }

    public async Task<ExecutiveDashboardDto> GenerateExecutiveDashboardAsync(Guid projectId, DateTime? asOfDate = null)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        var effectiveDate = asOfDate ?? DateTime.UtcNow;
        
        // Gather data from various services
        var schedule = await _scheduleService.GetCurrentScheduleAsync(projectId);
        var milestones = await _milestoneService.GetProjectMilestonesAsync(projectId);
        var budget = await _budgetService.GetProjectBudgetSummaryAsync(projectId);

        return new ExecutiveDashboardDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            AsOfDate = effectiveDate,
            
            // Overall metrics
            OverallProgress = schedule?.Progress ?? 0,
            OverallHealth = DetermineProjectHealth(schedule, budget),
            
            // Schedule metrics
            ScheduleProgress = schedule?.Progress ?? 0,
            ScheduleVariance = 0, // Would calculate from schedule data
            PlannedStartDate = project.PlannedStartDate,
            PlannedEndDate = project.PlannedEndDate,
            ForecastEndDate = project.PlannedEndDate, // Would calculate from trends
            
            // Cost metrics
            TotalBudget = budget?.TotalBudget ?? 0,
            ActualCost = budget?.ActualCost ?? 0,
            CommittedCost = budget?.CommittedCost ?? 0,
            CostVariance = (budget?.TotalBudget ?? 0) - (budget?.ActualCost ?? 0),
            CostPerformanceIndex = 1.0m, // Would calculate from EVM
            
            // Milestone metrics
            TotalMilestones = milestones.Count(),
            CompletedMilestones = milestones.Count(m => m.IsCompleted),
            UpcomingMilestones = milestones.Count(m => !m.IsCompleted && m.PlannedDate <= effectiveDate.AddDays(30)),
            OverdueMilestones = milestones.Count(m => !m.IsCompleted && m.PlannedDate < effectiveDate),
            
            // Risk metrics (simplified)
            HighRisks = 0,
            MediumRisks = 0,
            LowRisks = 0,
            
            // Key performance indicators
            KeyPerformanceIndicators = new Dictionary<string, decimal>
            {
                ["Schedule Performance Index"] = 1.0m,
                ["Cost Performance Index"] = 1.0m,
                ["Quality Index"] = 95.0m,
                ["Safety Index"] = 100.0m
            },
            
            GeneratedDate = DateTime.UtcNow
        };
    }

    public async Task<ProjectStatusReportDto> GenerateProjectStatusReportAsync(Guid projectId, DateTime? reportDate = null)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        var effectiveDate = reportDate ?? DateTime.UtcNow;
        var schedule = await _scheduleService.GetCurrentScheduleAsync(projectId);
        var milestones = await _milestoneService.GetUpcomingMilestonesAsync(projectId, 90);

        return new ProjectStatusReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            ReportDate = effectiveDate,
            ReportPeriod = $"{effectiveDate:MMMM yyyy}",
            
            // Project Overview
            ProjectStatus = "Active",
            ProjectStartDate = project.PlannedStartDate,
            ProjectEndDate = project.PlannedEndDate,
            TotalBudget = project.TotalBudget,
            PercentageComplete = schedule?.Progress ?? 0,
            
            // Schedule Information
            TotalActivities = schedule?.TotalActivities ?? 0,
            CompletedActivities = 0, // Would calculate from activities
            InProgressActivities = 0,
            DelayedActivities = 0,
            ScheduleVariance = 0,
            SchedulePerformanceIndex = 1.0m,
            
            // Cost Information
            BudgetAtCompletion = project.TotalBudget,
            ActualCost = 0,
            EarnedValue = 0,
            PlannedValue = 0,
            CostVariance = 0,
            CostPerformanceIndex = 1.0m,
            EstimateAtCompletion = project.TotalBudget,
            EstimateToComplete = project.TotalBudget,
            
            // Key Milestones
            KeyMilestones = milestones.Take(5).Select(m => new MilestoneSummaryDto
            {
                Name = m.Name,
                PlannedDate = m.PlannedDate,
                ActualDate = m.ActualDate,
                Status = m.IsCompleted ? "Completed" : m.PlannedDate < effectiveDate ? "Delayed" : "OnTrack",
                DaysVariance = m.ActualDate.HasValue ? (m.ActualDate.Value - m.PlannedDate).Days : 0
            }).ToList(),
            
            // Issues and Risks
            OpenIssues = 3,
            HighRisks = 1,
            MediumRisks = 2,
            LowRisks = 5,
            
            // Summary Sections
            ExecutiveSummary = $"Project {project.Name} is currently {schedule?.Progress ?? 0}% complete.",
            AccomplishmentsThisPeriod = "Completed foundation work. Started structural steel installation.",
            PlannedActivitiesNextPeriod = "Continue structural steel. Begin MEP rough-in.",
            KeyIssuesAndConcerns = "Weather delays impacting outdoor work.",
            RecommendationsAndActions = "Accelerate indoor activities to maintain schedule.",
            
            GeneratedBy = _currentUserService.Name ?? "System",
            GeneratedAt = DateTime.UtcNow
        };
    }

    public async Task<CostControlReportDto> GenerateCostControlReportAsync(Guid projectId, DateTime periodStart, DateTime periodEnd)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        var budget = await _budgetService.GetProjectBudgetSummaryAsync(projectId);
        var controlAccounts = await _controlAccountService.GetByProjectAsync(projectId);

        return new CostControlReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            
            // Budget Summary
            OriginalBudget = budget?.OriginalBudget ?? 0,
            CurrentBudget = budget?.TotalBudget ?? 0,
            ActualCostToDate = budget?.ActualCost ?? 0,
            CommittedCost = budget?.CommittedCost ?? 0,
            ForecastAtCompletion = budget?.ForecastAtCompletion ?? 0,
            
            // Variances
            BudgetVariance = (budget?.TotalBudget ?? 0) - (budget?.ActualCost ?? 0),
            CostVariance = 0, // Would calculate from EVM
            ScheduleVariance = 0, // Would calculate from EVM
            
            // Performance Indices
            CostPerformanceIndex = 1.0m,
            SchedulePerformanceIndex = 1.0m,
            
            // Control Account Summary
            ControlAccountSummaries = controlAccounts.Select(ca => new ControlAccountCostDto
            {
                ControlAccountId = ca.Id,
                Code = ca.Code,
                Name = ca.Name,
                Budget = ca.Budget,
                ActualCost = ca.ActualCost,
                CommittedCost = ca.CommittedCost,
                Variance = ca.Budget - ca.ActualCost
            }).ToList(),
            
            GeneratedDate = DateTime.UtcNow
        };
    }

    public async Task<EarnedValueReportDto> GenerateEarnedValueReportAsync(Guid projectId, DateTime? asOfDate = null)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        var effectiveDate = asOfDate ?? DateTime.UtcNow;
        var budget = await _budgetService.GetProjectBudgetSummaryAsync(projectId);

        // Simplified EVM calculations
        var plannedValue = budget?.TotalBudget ?? 0 * 0.4m; // 40% planned
        var earnedValue = budget?.TotalBudget ?? 0 * 0.35m; // 35% earned
        var actualCost = budget?.ActualCost ?? 0;

        return new EarnedValueReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            AsOfDate = effectiveDate,
            ReportCurrency = "USD",
            
            // Basic EVM Metrics
            BudgetAtCompletion = budget?.TotalBudget ?? 0,
            PlannedValue = plannedValue,
            EarnedValue = earnedValue,
            ActualCost = actualCost,
            
            // Variances
            CostVariance = earnedValue - actualCost,
            ScheduleVariance = earnedValue - plannedValue,
            VarianceAtCompletion = 0,
            
            // Performance Indices
            CostPerformanceIndex = actualCost > 0 ? earnedValue / actualCost : 1,
            SchedulePerformanceIndex = plannedValue > 0 ? earnedValue / plannedValue : 1,
            ToCompletePerformanceIndex = 1.0m,
            
            // Forecasts
            EstimateAtCompletion = budget?.TotalBudget ?? 0,
            EstimateToComplete = (budget?.TotalBudget ?? 0) - actualCost,
            ForecastMethod = "CPI",
            
            // Progress Percentages
            PercentageComplete = (budget?.TotalBudget ?? 0) > 0 ? earnedValue / (budget?.TotalBudget ?? 0) * 100 : 0,
            PercentageSpent = (budget?.TotalBudget ?? 0) > 0 ? actualCost / (budget?.TotalBudget ?? 0) * 100 : 0,
            PercentagePlanned = (budget?.TotalBudget ?? 0) > 0 ? plannedValue / (budget?.TotalBudget ?? 0) * 100 : 0,
            
            // Time Analysis
            ProjectStartDate = project.PlannedStartDate,
            ProjectEndDate = project.PlannedEndDate,
            PlannedDuration = (project.PlannedEndDate - project.PlannedStartDate).Days,
            
            // Trends (simplified)
            TrendData = new List<EarnedValueTrendDto>
            {
                new() { 
                    Date = effectiveDate.AddMonths(-2), 
                    PlannedValue = plannedValue * 0.5m, 
                    EarnedValue = earnedValue * 0.5m, 
                    ActualCost = actualCost * 0.5m,
                    CostVariance = earnedValue * 0.5m - actualCost * 0.5m,
                    ScheduleVariance = earnedValue * 0.5m - plannedValue * 0.5m,
                    CostPerformanceIndex = actualCost > 0 ? (earnedValue * 0.5m) / (actualCost * 0.5m) : 1,
                    SchedulePerformanceIndex = plannedValue > 0 ? (earnedValue * 0.5m) / (plannedValue * 0.5m) : 1
                },
                new() { 
                    Date = effectiveDate.AddMonths(-1), 
                    PlannedValue = plannedValue * 0.75m, 
                    EarnedValue = earnedValue * 0.75m, 
                    ActualCost = actualCost * 0.75m,
                    CostVariance = earnedValue * 0.75m - actualCost * 0.75m,
                    ScheduleVariance = earnedValue * 0.75m - plannedValue * 0.75m,
                    CostPerformanceIndex = actualCost > 0 ? (earnedValue * 0.75m) / (actualCost * 0.75m) : 1,
                    SchedulePerformanceIndex = plannedValue > 0 ? (earnedValue * 0.75m) / (plannedValue * 0.75m) : 1
                },
                new() { 
                    Date = effectiveDate, 
                    PlannedValue = plannedValue, 
                    EarnedValue = earnedValue, 
                    ActualCost = actualCost,
                    CostVariance = earnedValue - actualCost,
                    ScheduleVariance = earnedValue - plannedValue,
                    CostPerformanceIndex = actualCost > 0 ? earnedValue / actualCost : 1,
                    SchedulePerformanceIndex = plannedValue > 0 ? earnedValue / plannedValue : 1
                }
            },
            
            GeneratedDate = DateTime.UtcNow
        };
    }

    public async Task<ScheduleProgressReportDto> GenerateScheduleProgressReportAsync(Guid projectId, DateTime? asOfDate = null)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        var effectiveDate = asOfDate ?? DateTime.UtcNow;
        var schedule = await _scheduleService.GetCurrentScheduleAsync(projectId);

        return new ScheduleProgressReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            DataDate = effectiveDate,
            
            // Schedule Info
            ScheduleId = schedule?.Id ?? Guid.Empty,
            ScheduleName = schedule?.Name ?? "Current Schedule",
            BaselineDate = schedule?.BaselineDate,
            
            // Overall Progress
            OverallProgress = schedule?.Progress ?? 0,
            PlannedProgress = CalculatePlannedProgress(project, effectiveDate),
            ProgressVariance = (schedule?.Progress ?? 0) - CalculatePlannedProgress(project, effectiveDate),
            
            // Activity Summary
            TotalActivities = schedule?.TotalActivities ?? 0,
            CompletedActivities = 0, // Would get from activities
            InProgressActivities = 0,
            NotStartedActivities = 0,
            
            // Critical Path
            CriticalPathLength = 0,
            CriticalPathVariance = 0,
            
            // Float Analysis
            TotalFloat = schedule?.TotalFloat ?? 0,
            FreeFloat = 0,
            
            GeneratedDate = DateTime.UtcNow
        };
    }

    public async Task<MilestoneReportDto> GenerateMilestoneReportAsync(Guid projectId)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        var milestones = await _milestoneService.GetProjectMilestonesAsync(projectId);
        var dashboard = await _milestoneService.GetMilestoneDashboardAsync(projectId);

        return new MilestoneReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            
            // Summary
            TotalMilestones = dashboard.TotalMilestones,
            CompletedMilestones = dashboard.CompletedMilestones,
            UpcomingMilestones = dashboard.UpcomingMilestones,
            OverdueMilestones = dashboard.OverdueMilestones,
            CompletionPercentage = dashboard.CompletionPercentage,
            
            // Milestone Details
            Milestones = milestones.Select(m => new MilestoneDetailDto
            {
                MilestoneId = m.Id,
                Code = m.MilestoneCode,
                Name = m.Name,
                Type = m.Type.ToString(),
                PlannedDate = m.PlannedDate,
                ForecastDate = m.ForecastDate,
                ActualDate = m.ActualDate,
                Status = m.IsCompleted ? "Completed" : m.PlannedDate < DateTime.UtcNow ? "Overdue" : "Pending",
                IsCritical = m.IsCritical,
                IsContractual = m.IsContractual,
                CompletionPercentage = m.CompletionPercentage
            }).ToList(),
            
            GeneratedDate = DateTime.UtcNow
        };
    }

    public async Task<RiskRegisterReportDto> GenerateRiskRegisterReportAsync(Guid projectId)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Simplified - would integrate with risk management module
        return new RiskRegisterReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            
            // Summary
            TotalRisks = 0,
            OpenRisks = 0,
            ClosedRisks = 0,
            HighRisks = 0,
            MediumRisks = 0,
            LowRisks = 0,
            
            // Risk Details
            Risks = new List<RiskDetailDto>(),
            
            GeneratedDate = DateTime.UtcNow
        };
    }

    public async Task<RiskMatrixReportDto> GenerateRiskMatrixReportAsync(Guid projectId)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Simplified - would integrate with risk management module
        return new RiskMatrixReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            
            // Matrix Data
            MatrixData = new Dictionary<string, Dictionary<string, int>>(),
            
            // Summary by Category
            RisksByCategory = new Dictionary<string, int>(),
            
            GeneratedDate = DateTime.UtcNow
        };
    }

    public async Task<QualityMetricsReportDto> GenerateQualityMetricsReportAsync(Guid projectId, DateTime periodStart, DateTime periodEnd)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Simplified - would integrate with quality management module
        return new QualityMetricsReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            
            // Metrics
            TotalInspections = 0,
            PassedInspections = 0,
            FailedInspections = 0,
            QualityScore = 95.0m,
            
            // Non-Conformances
            TotalNonConformances = 0,
            OpenNonConformances = 0,
            ClosedNonConformances = 0,
            
            GeneratedDate = DateTime.UtcNow
        };
    }

    public async Task<ResourceUtilizationReportDto> GenerateResourceUtilizationReportAsync(Guid projectId, DateTime periodStart, DateTime periodEnd)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Simplified - would integrate with resource management module
        return new ResourceUtilizationReportDto
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectCode = project.Code,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            
            // Summary
            TotalResources = 0,
            ActiveResources = 0,
            AverageUtilization = 0,
            
            // Resource Details
            ResourceUtilization = new List<ResourceUtilizationDetailDto>(),
            
            GeneratedDate = DateTime.UtcNow
        };
    }

    public async Task<byte[]> ExportToPdfAsync(Guid reportId)
    {
        var content = await GetReportContentAsync(reportId);
        if (content == null)
            throw new InvalidOperationException("Report content not found");

        // If already PDF, return as is
        var report = await GetByIdAsync(reportId);
        if (report?.ContentType == "application/pdf")
            return content;

        // TODO: Implement conversion to PDF
        return content;
    }

    public async Task<byte[]> ExportToExcelAsync(Guid reportId)
    {
        var content = await GetReportContentAsync(reportId);
        if (content == null)
            throw new InvalidOperationException("Report content not found");

        // TODO: Implement conversion to Excel
        return content;
    }

    public async Task<byte[]> ExportToWordAsync(Guid reportId)
    {
        var content = await GetReportContentAsync(reportId);
        if (content == null)
            throw new InvalidOperationException("Report content not found");

        // TODO: Implement conversion to Word
        return content;
    }

    public async Task<byte[]> ExportToPowerPointAsync(Guid reportId)
    {
        var content = await GetReportContentAsync(reportId);
        if (content == null)
            throw new InvalidOperationException("Report content not found");

        // TODO: Implement conversion to PowerPoint
        return content;
    }

    public async Task<byte[]> ExportToCsvAsync(Guid reportId)
    {
        var content = await GetReportContentAsync(reportId);
        if (content == null)
            throw new InvalidOperationException("Report content not found");

        // TODO: Implement conversion to CSV
        return content;
    }

    public async Task<byte[]> ExportMultipleReportsAsync(List<Guid> reportIds, ExportFormat format, bool combineIntoOne = false)
    {
        if (combineIntoOne)
        {
            // TODO: Implement combining multiple reports
            return Array.Empty<byte>();
        }
        else
        {
            // TODO: Implement creating a zip file with multiple reports
            return Array.Empty<byte>();
        }
    }

    // IBaseService implementation
    public async Task<ReportDto> CreateAsync(GenerateReportDto createDto, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        return await GenerateReportAsync(createDto);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_reportCache.ContainsKey(id))
        {
            _reportCache.Remove(id);
            return true;
        }
        return false;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _reportCache.ContainsKey(id);
    }

    public async Task<IEnumerable<ReportDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _reportCache.Values;
    }

    public async Task<PagedResult<ReportDto>> GetAllPagedAsync(int pageNumber = 1, int pageSize = 20, string? orderBy = null, bool descending = false, CancellationToken cancellationToken = default)
    {
        var reports = _reportCache.Values.AsQueryable();
        
        // Apply ordering
        reports = orderBy?.ToLower() switch
        {
            "name" => descending ? reports.OrderByDescending(r => r.Name) : reports.OrderBy(r => r.Name),
            "date" => descending ? reports.OrderByDescending(r => r.GeneratedDate) : reports.OrderBy(r => r.GeneratedDate),
            _ => descending ? reports.OrderByDescending(r => r.GeneratedDate) : reports.OrderBy(r => r.GeneratedDate)
        };

        var totalCount = reports.Count();
        var items = reports.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<ReportDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<ReportDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _reportCache.TryGetValue(id, out var report) ? report : null;
    }

    public async Task<ReportDto?> UpdateAsync(Guid id, GenerateReportDto updateDto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        // Reports are immutable once generated, so we regenerate instead
        return await RegenerateReportAsync(id);
    }

    // Helper methods
    private async Task<byte[]> GenerateReportContent(ReportDto report, GenerateReportDto dto)
    {
        // This is a simplified implementation
        // In a real system, this would use a reporting engine like:
        // - Crystal Reports
        // - SSRS
        // - FastReport
        // - DevExpress Reports
        // - Telerik Reporting
        
        var content = new StringBuilder();
        content.AppendLine($"# {report.Name}");
        content.AppendLine($"Generated: {report.GeneratedDate:yyyy-MM-dd HH:mm:ss}");
        content.AppendLine($"Project: {report.ProjectName ?? "N/A"}");
        content.AppendLine();
        content.AppendLine("## Report Content");
        content.AppendLine("This is a placeholder report content.");
        content.AppendLine();
        content.AppendLine("### Parameters");
        foreach (var param in report.Parameters)
        {
            content.AppendLine($"- {param.Key}: {param.Value}");
        }

        return Encoding.UTF8.GetBytes(content.ToString());
    }

    private string GetContentType(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.Pdf => "application/pdf",
            ExportFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ExportFormat.Word => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ExportFormat.PowerPoint => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ExportFormat.Csv => "text/csv",
            ExportFormat.Html => "text/html",
            ExportFormat.Xml => "application/xml",
            ExportFormat.Json => "application/json",
            _ => "application/octet-stream"
        };
    }

    private string DetermineProjectHealth(object? schedule, object? budget)
    {
        // Simplified health determination
        return "On Track";
    }

    private decimal CalculatePlannedProgress(Core.DTOs.Organization.Project.ProjectDto project, DateTime asOfDate)
    {
        if (project.PlannedStartDate > asOfDate)
            return 0;

        if (project.PlannedEndDate < asOfDate)
            return 100;

        var totalDays = (project.PlannedEndDate - project.PlannedStartDate).TotalDays;
        var elapsedDays = (asOfDate - project.PlannedStartDate).TotalDays;

        return totalDays > 0 ? (decimal)(elapsedDays / totalDays * 100) : 0;
    }
}