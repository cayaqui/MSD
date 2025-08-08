using Application.Interfaces.Visualization;
using Application.Interfaces.Common;
using Application.Interfaces.Progress;
using Application.Interfaces.Cost;
using Application.Interfaces.Contracts;
// using Application.Interfaces.Resources; // TODO: Implement IResourceService
using Core.DTOs.Visualization;
using Domain.Entities.Progress;
// using Domain.Entities.Planning; // Namespace doesn't exist
using Domain.Entities.Organization;
using Domain.Entities.Cost;
using Domain.Entities.Contracts.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Visualization;

public class VisualizationService : IVisualizationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScheduleService _scheduleService;
    private readonly IEVMService _evmService;
    private readonly IBudgetService _budgetService;
    // private readonly IResourceService _resourceService; // TODO: Implement IResourceService
    private readonly ILogger<VisualizationService> _logger;

    public VisualizationService(
        IUnitOfWork unitOfWork,
        IScheduleService scheduleService,
        IEVMService evmService,
        IBudgetService budgetService,
        // IResourceService resourceService, // TODO: Implement IResourceService
        ILogger<VisualizationService> logger)
    {
        _unitOfWork = unitOfWork;
        _scheduleService = scheduleService;
        _evmService = evmService;
        _budgetService = budgetService;
        // _resourceService = resourceService; // TODO: Implement IResourceService
        _logger = logger;
    }

    #region Gantt Chart Methods

    public async Task<GanttChartDto> GenerateProjectGanttAsync(Guid projectId, GanttConfigDto? config = null)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>()
                .GetAsync(p => p.Id == projectId && !p.IsDeleted);

            if (project == null)
                throw new InvalidOperationException("Project not found");

            config ??= new GanttConfigDto();

            var activities = await _unitOfWork.Repository<Activity>()
                .GetAllAsync(
                    filter: a => a.ProjectId == projectId && !a.IsDeleted,
                    includeProperties: "WBSItem,Resources,Predecessors,Successors");

            var milestones = await _unitOfWork.Repository<Milestone>()
                .GetAllAsync(
                    filter: m => m.ProjectId == projectId && !m.IsDeleted);

            var ganttChart = new GanttChartDto
            {
                Title = $"{project.ProjectName} - Project Schedule",
                StartDate = project.PlannedStartDate,
                EndDate = project.PlannedEndDate,
                Config = config
            };

            // Convert activities to Gantt tasks
            foreach (var activity in activities)
            {
                var task = new GanttTaskDto
                {
                    Id = activity.Id.ToString(),
                    Name = activity.ActivityName,
                    ParentId = activity.WBSItem?.ParentId?.ToString() ?? string.Empty,
                    StartDate = activity.EarlyStart ?? activity.PlannedStart,
                    EndDate = activity.EarlyFinish ?? activity.PlannedFinish,
                    Progress = activity.PercentComplete,
                    AssignedTo = string.Join(", ", activity.Resources?.Select(r => r.ResourceName) ?? new List<string>()),
                    Type = activity.IsMilestone ? "milestone" : "task",
                    IsCritical = activity.IsCritical,
                    Dependencies = activity.Predecessors?.Select(p => p.PredecessorId.ToString()).ToList() ?? new List<string>(),
                    Color = activity.IsCritical ? "#FF0000" : "#0066CC"
                };

                ganttChart.Tasks.Add(task);
            }

            // Add milestones
            foreach (var milestone in milestones)
            {
                var ganttMilestone = new GanttMilestoneDto
                {
                    Id = milestone.Id.ToString(),
                    Name = milestone.MilestoneName,
                    Date = milestone.PlannedDate,
                    Type = milestone.Type.ToString(),
                    IsComplete = milestone.IsComplete,
                    Color = milestone.IsComplete ? "#00CC00" : "#FFA500"
                };

                ganttChart.Milestones.Add(ganttMilestone);
            }

            // Calculate critical path
            ganttChart.CriticalPath = activities
                .Where(a => a.IsCritical)
                .Select(a => a.Id.ToString())
                .ToList();

            return ganttChart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate project Gantt chart");
            throw;
        }
    }

    public async Task<GanttChartDto> GenerateWBSGanttAsync(Guid projectId, string? wbsFilter = null, GanttConfigDto? config = null)
    {
        try
        {
            config ??= new GanttConfigDto();

            var wbsItems = await _unitOfWork.Repository<WBSItem>()
                .GetAllAsync(
                    filter: w => w.ProjectId == projectId && !w.IsDeleted,
                    includeProperties: "Activities");

            if (!string.IsNullOrEmpty(wbsFilter))
            {
                wbsItems = wbsItems.Where(w => w.WBSCode.StartsWith(wbsFilter));
            }

            var ganttChart = new GanttChartDto
            {
                Title = "Work Breakdown Structure",
                Config = config
            };

            DateTime? minDate = null;
            DateTime? maxDate = null;

            // Create hierarchical structure
            foreach (var wbsItem in wbsItems.OrderBy(w => w.WBSCode))
            {
                var activities = wbsItem.Activities?.Where(a => !a.IsDeleted) ?? new List<Activity>();
                
                if (activities.Any() || wbsItem.IsPackage)
                {
                    var startDate = activities.Any() 
                        ? activities.Min(a => a.PlannedStart) 
                        : wbsItem.PlannedStartDate ?? DateTime.UtcNow;
                    
                    var endDate = activities.Any() 
                        ? activities.Max(a => a.PlannedFinish) 
                        : wbsItem.PlannedEndDate ?? DateTime.UtcNow.AddDays(30);

                    var task = new GanttTaskDto
                    {
                        Id = wbsItem.Id.ToString(),
                        Name = $"{wbsItem.WBSCode} - {wbsItem.WBSName}",
                        ParentId = wbsItem.ParentId?.ToString() ?? string.Empty,
                        StartDate = startDate,
                        EndDate = endDate,
                        Progress = wbsItem.ProgressPercentage,
                        Type = activities.Any() ? "summary" : "task",
                        Color = wbsItem.Level switch
                        {
                            1 => "#1E3A8A",
                            2 => "#2563EB",
                            3 => "#3B82F6",
                            _ => "#60A5FA"
                        }
                    };

                    ganttChart.Tasks.Add(task);

                    // Update date range
                    minDate = minDate == null || startDate < minDate ? startDate : minDate;
                    maxDate = maxDate == null || endDate > maxDate ? endDate : maxDate;
                }
            }

            ganttChart.StartDate = minDate ?? DateTime.UtcNow;
            ganttChart.EndDate = maxDate ?? DateTime.UtcNow.AddMonths(1);

            return ganttChart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate WBS Gantt chart");
            throw;
        }
    }

    public async Task<GanttChartDto> GenerateScheduleGanttAsync(Guid scheduleId, GanttConfigDto? config = null)
    {
        try
        {
            var schedule = await _unitOfWork.Repository<Schedule>()
                .GetAsync(
                    filter: s => s.Id == scheduleId && !s.IsDeleted,
                    includeProperties: "Project,Activities");

            if (schedule == null)
                throw new InvalidOperationException("Schedule not found");

            config ??= new GanttConfigDto();

            var ganttChart = new GanttChartDto
            {
                Title = $"{schedule.ScheduleName} - Rev {schedule.Revision}",
                StartDate = schedule.ScheduleStartDate,
                EndDate = schedule.ScheduleEndDate,
                Config = config
            };

            foreach (var activity in schedule.Activities.Where(a => !a.IsDeleted))
            {
                var task = new GanttTaskDto
                {
                    Id = activity.Id.ToString(),
                    Name = activity.ActivityName,
                    StartDate = activity.PlannedStart,
                    EndDate = activity.PlannedFinish,
                    Progress = activity.PercentComplete,
                    Type = activity.IsMilestone ? "milestone" : "task",
                    IsCritical = activity.IsCritical,
                    Color = activity.Status switch
                    {
                        "Completed" => "#00CC00",
                        "InProgress" => "#FFA500",
                        "Delayed" => "#FF0000",
                        _ => "#0066CC"
                    }
                };

                ganttChart.Tasks.Add(task);
            }

            return ganttChart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate schedule Gantt chart");
            throw;
        }
    }

    public async Task<GanttChartDto> GenerateContractGanttAsync(Guid contractId, GanttConfigDto? config = null)
    {
        try
        {
            var contract = await _unitOfWork.Repository<Contract>()
                .GetAsync(
                    filter: c => c.Id == contractId && !c.IsDeleted,
                    includeProperties: "Milestones");

            if (contract == null)
                throw new InvalidOperationException("Contract not found");

            config ??= new GanttConfigDto();

            var ganttChart = new GanttChartDto
            {
                Title = $"{contract.ContractNumber} - Contract Schedule",
                StartDate = contract.StartDate,
                EndDate = contract.CurrentEndDate,
                Config = config
            };

            // Add contract milestones
            foreach (var milestone in contract.Milestones.Where(m => !m.IsDeleted))
            {
                var task = new GanttTaskDto
                {
                    Id = milestone.Id.ToString(),
                    Name = milestone.MilestoneName,
                    StartDate = milestone.PlannedDate,
                    EndDate = milestone.PlannedDate,
                    Progress = milestone.ProgressPercentage,
                    Type = "milestone",
                    IsCritical = milestone.IsCritical,
                    Color = milestone.Status switch
                    {
                        "Completed" => "#00CC00",
                        "Approved" => "#0066CC",
                        "InProgress" => "#FFA500",
                        _ => "#CCCCCC"
                    }
                };

                ganttChart.Tasks.Add(task);

                // Also add to milestones collection
                ganttChart.Milestones.Add(new GanttMilestoneDto
                {
                    Id = milestone.Id.ToString(),
                    Name = milestone.MilestoneName,
                    Date = milestone.PlannedDate,
                    Type = milestone.Type.ToString(),
                    IsComplete = milestone.Status == "Completed",
                    Color = task.Color
                });
            }

            return ganttChart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate contract Gantt chart");
            throw;
        }
    }

    public async Task<GanttChartDto> GenerateResourceGanttAsync(Guid projectId, List<Guid>? resourceIds = null, GanttConfigDto? config = null)
    {
        try
        {
            config ??= new GanttConfigDto();

            var resourceQuery = _unitOfWork.Repository<Resource>()
                .Query()
                .Where(r => r.ProjectId == projectId && !r.IsDeleted);

            if (resourceIds != null && resourceIds.Any())
            {
                resourceQuery = resourceQuery.Where(r => resourceIds.Contains(r.Id));
            }

            var resources = await resourceQuery
                .Include(r => r.Assignments)
                .ThenInclude(a => a.Activity)
                .ToListAsync();

            var ganttChart = new GanttChartDto
            {
                Title = "Resource Allocation",
                Config = config
            };

            DateTime? minDate = null;
            DateTime? maxDate = null;

            foreach (var resource in resources)
            {
                // Resource summary row
                var resourceTask = new GanttTaskDto
                {
                    Id = resource.Id.ToString(),
                    Name = resource.ResourceName,
                    Type = "summary",
                    Color = "#1E3A8A"
                };

                var assignments = resource.Assignments.Where(a => !a.IsDeleted && a.Activity != null);
                
                if (assignments.Any())
                {
                    resourceTask.StartDate = assignments.Min(a => a.Activity.PlannedStart);
                    resourceTask.EndDate = assignments.Max(a => a.Activity.PlannedFinish);
                    
                    minDate = minDate == null || resourceTask.StartDate < minDate ? resourceTask.StartDate : minDate;
                    maxDate = maxDate == null || resourceTask.EndDate > maxDate ? resourceTask.EndDate : maxDate;

                    ganttChart.Tasks.Add(resourceTask);

                    // Add individual assignments
                    foreach (var assignment in assignments)
                    {
                        var assignmentTask = new GanttTaskDto
                        {
                            Id = assignment.Id.ToString(),
                            Name = assignment.Activity.ActivityName,
                            ParentId = resource.Id.ToString(),
                            StartDate = assignment.Activity.PlannedStart,
                            EndDate = assignment.Activity.PlannedFinish,
                            Progress = assignment.Activity.PercentComplete,
                            Type = "task",
                            Color = assignment.Units > 100 ? "#FF0000" : "#0066CC",
                            CustomData = new Dictionary<string, object>
                            {
                                { "Units", assignment.Units },
                                { "Role", assignment.Role ?? "N/A" }
                            }
                        };

                        ganttChart.Tasks.Add(assignmentTask);
                    }
                }
            }

            ganttChart.StartDate = minDate ?? DateTime.UtcNow;
            ganttChart.EndDate = maxDate ?? DateTime.UtcNow.AddMonths(1);

            return ganttChart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate resource Gantt chart");
            throw;
        }
    }

    #endregion

    #region S-Curve Methods

    public async Task<CostSCurveDto> GenerateCostSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>()
                .GetAsync(p => p.Id == projectId && !p.IsDeleted);

            if (project == null)
                throw new InvalidOperationException("Project not found");

            var budget = await _budgetService.GetByProjectAsync(projectId);
            if (budget == null)
                throw new InvalidOperationException("Project budget not found");

            startDate ??= project.PlannedStartDate;
            endDate ??= project.PlannedEndDate;

            var sCurve = new CostSCurveDto
            {
                Title = $"{project.ProjectName} - Cost S-Curve",
                XAxisLabel = "Time",
                YAxisLabel = "Cost ($)",
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                TotalBudget = budget.TotalBudget,
                CurrentCost = budget.ActualCost ?? 0,
                DataDate = DateTime.UtcNow,
                Config = new SCurveConfigDto
                {
                    ChartType = "cost",
                    ShowCumulative = true,
                    ValueFormat = "currency"
                }
            };

            // Planned Cost (BCWS)
            var plannedSeries = new SCurveSeriesDto
            {
                Name = "Planned Cost (BCWS)",
                Type = "line",
                Color = "#0066CC",
                LineStyle = "solid"
            };

            // Actual Cost (ACWP)
            var actualSeries = new SCurveSeriesDto
            {
                Name = "Actual Cost (ACWP)",
                Type = "line",
                Color = "#00CC00",
                LineStyle = "solid"
            };

            // Earned Value (BCWP)
            var earnedSeries = new SCurveSeriesDto
            {
                Name = "Earned Value (BCWP)",
                Type = "line",
                Color = "#FFA500",
                LineStyle = "dashed"
            };

            // Forecast Cost
            var forecastSeries = new SCurveSeriesDto
            {
                Name = "Forecast at Completion",
                Type = "line",
                Color = "#FF0000",
                LineStyle = "dotted"
            };

            // Generate data points
            var currentDate = startDate.Value;
            var monthlyPlannedCost = budget.TotalBudget / ((endDate.Value - startDate.Value).Days / 30.0m);
            decimal cumulativePlanned = 0;
            decimal cumulativeActual = 0;
            decimal cumulativeEarned = 0;

            while (currentDate <= endDate.Value)
            {
                // Calculate planned cost (linear distribution for now)
                cumulativePlanned += monthlyPlannedCost;
                plannedSeries.DataPoints.Add(new SCurveDataPointDto
                {
                    Date = currentDate,
                    Value = monthlyPlannedCost,
                    CumulativeValue = cumulativePlanned
                });

                // Get actual costs up to current date
                if (currentDate <= DateTime.UtcNow)
                {
                    // This would fetch real actual cost data
                    var monthlyActual = monthlyPlannedCost * 0.95m; // Simulated
                    cumulativeActual += monthlyActual;
                    actualSeries.DataPoints.Add(new SCurveDataPointDto
                    {
                        Date = currentDate,
                        Value = monthlyActual,
                        CumulativeValue = cumulativeActual
                    });

                    // Calculate earned value
                    var earnedPercentage = 0.9m; // Simulated
                    var monthlyEarned = monthlyPlannedCost * earnedPercentage;
                    cumulativeEarned += monthlyEarned;
                    earnedSeries.DataPoints.Add(new SCurveDataPointDto
                    {
                        Date = currentDate,
                        Value = monthlyEarned,
                        CumulativeValue = cumulativeEarned
                    });
                }

                // Forecast
                if (currentDate >= DateTime.UtcNow)
                {
                    var forecastValue = cumulativeActual + (budget.TotalBudget - cumulativeEarned) * 1.1m;
                    forecastSeries.DataPoints.Add(new SCurveDataPointDto
                    {
                        Date = currentDate,
                        Value = 0,
                        CumulativeValue = forecastValue
                    });
                }

                currentDate = currentDate.AddMonths(1);
            }

            sCurve.Series.Add(plannedSeries);
            sCurve.Series.Add(actualSeries);
            sCurve.Series.Add(earnedSeries);
            sCurve.Series.Add(forecastSeries);

            sCurve.ForecastCost = forecastSeries.DataPoints.LastOrDefault()?.CumulativeValue ?? budget.TotalBudget;

            return sCurve;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate cost S-curve");
            throw;
        }
    }

    public async Task<ProgressSCurveDto> GenerateProgressSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>()
                .GetAsync(p => p.Id == projectId && !p.IsDeleted);

            if (project == null)
                throw new InvalidOperationException("Project not found");

            startDate ??= project.PlannedStartDate;
            endDate ??= project.PlannedEndDate;

            var sCurve = new ProgressSCurveDto
            {
                Title = $"{project.ProjectName} - Progress S-Curve",
                XAxisLabel = "Time",
                YAxisLabel = "Progress (%)",
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                DataDate = DateTime.UtcNow,
                Config = new SCurveConfigDto
                {
                    ChartType = "progress",
                    ShowCumulative = true,
                    ValueFormat = "percentage"
                }
            };

            // Planned Progress
            var plannedSeries = new SCurveSeriesDto
            {
                Name = "Planned Progress",
                Type = "line",
                Color = "#0066CC",
                LineStyle = "solid"
            };

            // Actual Progress
            var actualSeries = new SCurveSeriesDto
            {
                Name = "Actual Progress",
                Type = "line",
                Color = "#00CC00",
                LineStyle = "solid"
            };

            // Forecast Progress
            var forecastSeries = new SCurveSeriesDto
            {
                Name = "Forecast Progress",
                Type = "line",
                Color = "#FFA500",
                LineStyle = "dashed"
            };

            // Get activities
            var activities = await _unitOfWork.Repository<Activity>()
                .GetAllAsync(a => a.ProjectId == projectId && !a.IsDeleted);

            // Generate data points
            var currentDate = startDate.Value;
            var totalDuration = (endDate.Value - startDate.Value).Days;

            while (currentDate <= endDate.Value)
            {
                var elapsed = (currentDate - startDate.Value).Days;
                var plannedProgress = Math.Min(100, (decimal)elapsed / totalDuration * 100);

                plannedSeries.DataPoints.Add(new SCurveDataPointDto
                {
                    Date = currentDate,
                    Value = plannedProgress
                });

                if (currentDate <= DateTime.UtcNow)
                {
                    // Calculate actual progress based on activities
                    var completedActivities = activities.Where(a => 
                        a.ActualFinish.HasValue && a.ActualFinish.Value <= currentDate);
                    var actualProgress = activities.Any() 
                        ? (decimal)completedActivities.Count() / activities.Count() * 100 
                        : 0;

                    actualSeries.DataPoints.Add(new SCurveDataPointDto
                    {
                        Date = currentDate,
                        Value = actualProgress
                    });
                }

                if (currentDate >= DateTime.UtcNow)
                {
                    // Forecast based on current performance
                    var currentProgress = actualSeries.DataPoints.LastOrDefault()?.Value ?? 0;
                    var remainingTime = (endDate.Value - DateTime.UtcNow).Days;
                    var remainingProgress = 100 - currentProgress;
                    var dailyRate = remainingTime > 0 ? remainingProgress / remainingTime : 0;
                    var forecastProgress = currentProgress + dailyRate * (currentDate - DateTime.UtcNow).Days;

                    forecastSeries.DataPoints.Add(new SCurveDataPointDto
                    {
                        Date = currentDate,
                        Value = Math.Min(100, forecastProgress)
                    });
                }

                currentDate = currentDate.AddMonths(1);
            }

            sCurve.Series.Add(plannedSeries);
            sCurve.Series.Add(actualSeries);
            sCurve.Series.Add(forecastSeries);

            sCurve.PlannedProgress = plannedSeries.DataPoints.LastOrDefault(p => p.Date <= DateTime.UtcNow)?.Value ?? 0;
            sCurve.ActualProgress = actualSeries.DataPoints.LastOrDefault()?.Value ?? 0;
            sCurve.ForecastProgress = forecastSeries.DataPoints.LastOrDefault()?.Value ?? 0;

            return sCurve;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate progress S-curve");
            throw;
        }
    }

    public async Task<SCurveDto> GenerateEarnedValueSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var evmData = await _evmService.CalculateEVMAsync(projectId);
            
            var project = await _unitOfWork.Repository<Project>()
                .GetAsync(p => p.Id == projectId && !p.IsDeleted);

            startDate ??= project.PlannedStartDate;
            endDate ??= project.PlannedEndDate;

            var sCurve = new SCurveDto
            {
                Title = $"{project.ProjectName} - Earned Value Analysis",
                XAxisLabel = "Time",
                YAxisLabel = "Value ($)",
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                Config = new SCurveConfigDto
                {
                    ChartType = "cost",
                    ShowCumulative = true,
                    ValueFormat = "currency",
                    ShowVariance = true
                }
            };

            // Create series for PV, EV, AC
            var pvSeries = new SCurveSeriesDto
            {
                Name = "Planned Value (PV)",
                Type = "line",
                Color = "#0066CC"
            };

            var evSeries = new SCurveSeriesDto
            {
                Name = "Earned Value (EV)",
                Type = "line",
                Color = "#00CC00"
            };

            var acSeries = new SCurveSeriesDto
            {
                Name = "Actual Cost (AC)",
                Type = "line",
                Color = "#FF0000"
            };

            // Add current values as data points
            var currentDate = DateTime.UtcNow;
            pvSeries.DataPoints.Add(new SCurveDataPointDto
            {
                Date = currentDate,
                Value = evmData.PlannedValue,
                CumulativeValue = evmData.PlannedValue
            });

            evSeries.DataPoints.Add(new SCurveDataPointDto
            {
                Date = currentDate,
                Value = evmData.EarnedValue,
                CumulativeValue = evmData.EarnedValue
            });

            acSeries.DataPoints.Add(new SCurveDataPointDto
            {
                Date = currentDate,
                Value = evmData.ActualCost,
                CumulativeValue = evmData.ActualCost
            });

            sCurve.Series.Add(pvSeries);
            sCurve.Series.Add(evSeries);
            sCurve.Series.Add(acSeries);

            return sCurve;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate earned value S-curve");
            throw;
        }
    }

    public async Task<SCurveDto> GenerateResourceSCurveAsync(Guid projectId, string resourceType, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>()
                .GetAsync(p => p.Id == projectId && !p.IsDeleted);

            if (project == null)
                throw new InvalidOperationException("Project not found");

            startDate ??= project.PlannedStartDate;
            endDate ??= project.PlannedEndDate;

            var sCurve = new SCurveDto
            {
                Title = $"{project.ProjectName} - {resourceType} Resource Curve",
                XAxisLabel = "Time",
                YAxisLabel = "Resource Units",
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                Config = new SCurveConfigDto
                {
                    ChartType = "resource",
                    ShowCumulative = false,
                    ValueFormat = "number"
                }
            };

            // Get resources of specified type
            var resources = await _unitOfWork.Repository<Resource>()
                .GetAllAsync(
                    filter: r => r.ProjectId == projectId && 
                               r.ResourceType == resourceType && 
                               !r.IsDeleted,
                    includeProperties: "Assignments.Activity");

            var plannedSeries = new SCurveSeriesDto
            {
                Name = $"Planned {resourceType}",
                Type = "area",
                Color = "#0066CC",
                Fill = true
            };

            var actualSeries = new SCurveSeriesDto
            {
                Name = $"Actual {resourceType}",
                Type = "line",
                Color = "#00CC00"
            };

            // Generate resource histogram data
            var currentDate = startDate.Value;
            while (currentDate <= endDate.Value)
            {
                decimal plannedUnits = 0;
                decimal actualUnits = 0;

                foreach (var resource in resources)
                {
                    var assignments = resource.Assignments
                        .Where(a => a.Activity != null &&
                                   a.Activity.PlannedStart <= currentDate &&
                                   a.Activity.PlannedFinish >= currentDate);

                    plannedUnits += assignments.Sum(a => a.Units);

                    if (currentDate <= DateTime.UtcNow)
                    {
                        var actualAssignments = assignments
                            .Where(a => a.Activity.ActualStart.HasValue &&
                                       a.Activity.ActualStart.Value <= currentDate);
                        actualUnits += actualAssignments.Sum(a => a.Units);
                    }
                }

                plannedSeries.DataPoints.Add(new SCurveDataPointDto
                {
                    Date = currentDate,
                    Value = plannedUnits
                });

                if (currentDate <= DateTime.UtcNow)
                {
                    actualSeries.DataPoints.Add(new SCurveDataPointDto
                    {
                        Date = currentDate,
                        Value = actualUnits
                    });
                }

                currentDate = currentDate.AddWeeks(1);
            }

            sCurve.Series.Add(plannedSeries);
            sCurve.Series.Add(actualSeries);

            return sCurve;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate resource S-curve");
            throw;
        }
    }

    public async Task<SCurveDto> GenerateCashFlowSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var project = await _unitOfWork.Repository<Project>()
                .GetAsync(p => p.Id == projectId && !p.IsDeleted);

            if (project == null)
                throw new InvalidOperationException("Project not found");

            startDate ??= project.PlannedStartDate;
            endDate ??= project.PlannedEndDate;

            var sCurve = new SCurveDto
            {
                Title = $"{project.ProjectName} - Cash Flow",
                XAxisLabel = "Time",
                YAxisLabel = "Cash Flow ($)",
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                Config = new SCurveConfigDto
                {
                    ChartType = "cashflow",
                    ShowCumulative = true,
                    ValueFormat = "currency"
                }
            };

            // Income series
            var incomeSeries = new SCurveSeriesDto
            {
                Name = "Income",
                Type = "bar",
                Color = "#00CC00"
            };

            // Expense series
            var expenseSeries = new SCurveSeriesDto
            {
                Name = "Expenses",
                Type = "bar",
                Color = "#FF0000"
            };

            // Net cash flow series
            var netCashFlowSeries = new SCurveSeriesDto
            {
                Name = "Net Cash Flow",
                Type = "line",
                Color = "#0066CC",
                LineWidth = 3
            };

            // Cumulative cash flow
            var cumulativeSeries = new SCurveSeriesDto
            {
                Name = "Cumulative Cash Flow",
                Type = "area",
                Color = "#FFA500",
                Fill = true
            };

            // Generate cash flow data
            decimal cumulative = 0;
            var currentDate = startDate.Value;

            while (currentDate <= endDate.Value)
            {
                // This would fetch real financial data
                var monthlyIncome = 100000m; // Simulated
                var monthlyExpenses = 85000m; // Simulated
                var netCashFlow = monthlyIncome - monthlyExpenses;
                cumulative += netCashFlow;

                incomeSeries.DataPoints.Add(new SCurveDataPointDto
                {
                    Date = currentDate,
                    Value = monthlyIncome
                });

                expenseSeries.DataPoints.Add(new SCurveDataPointDto
                {
                    Date = currentDate,
                    Value = -monthlyExpenses
                });

                netCashFlowSeries.DataPoints.Add(new SCurveDataPointDto
                {
                    Date = currentDate,
                    Value = netCashFlow
                });

                cumulativeSeries.DataPoints.Add(new SCurveDataPointDto
                {
                    Date = currentDate,
                    Value = cumulative,
                    CumulativeValue = cumulative
                });

                currentDate = currentDate.AddMonths(1);
            }

            sCurve.Series.Add(incomeSeries);
            sCurve.Series.Add(expenseSeries);
            sCurve.Series.Add(netCashFlowSeries);
            sCurve.Series.Add(cumulativeSeries);

            return sCurve;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate cash flow S-curve");
            throw;
        }
    }

    public async Task<SCurveDto> GenerateContractSCurveAsync(Guid contractId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var contract = await _unitOfWork.Repository<Contract>()
                .GetAsync(
                    filter: c => c.Id == contractId && !c.IsDeleted,
                    includeProperties: "Milestones,Valuations");

            if (contract == null)
                throw new InvalidOperationException("Contract not found");

            startDate ??= contract.StartDate;
            endDate ??= contract.CurrentEndDate;

            var sCurve = new SCurveDto
            {
                Title = $"{contract.ContractNumber} - Contract Value Curve",
                XAxisLabel = "Time",
                YAxisLabel = "Contract Value ($)",
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                Config = new SCurveConfigDto
                {
                    ChartType = "cost",
                    ShowCumulative = true,
                    ValueFormat = "currency"
                }
            };

            // Planned value series
            var plannedSeries = new SCurveSeriesDto
            {
                Name = "Planned Contract Value",
                Type = "line",
                Color = "#0066CC"
            };

            // Actual value series
            var actualSeries = new SCurveSeriesDto
            {
                Name = "Actual Contract Value",
                Type = "line",
                Color = "#00CC00"
            };

            // Invoiced series
            var invoicedSeries = new SCurveSeriesDto
            {
                Name = "Invoiced Amount",
                Type = "line",
                Color = "#FFA500",
                LineStyle = "dashed"
            };

            // Paid series
            var paidSeries = new SCurveSeriesDto
            {
                Name = "Paid Amount",
                Type = "area",
                Color = "#90EE90",
                Fill = true
            };

            // Generate data based on milestones and valuations
            decimal cumulativePlanned = 0;
            decimal cumulativeActual = 0;
            decimal cumulativeInvoiced = 0;
            decimal cumulativePaid = 0;

            var currentDate = startDate.Value;
            while (currentDate <= endDate.Value)
            {
                // Planned value from milestones
                var plannedMilestones = contract.Milestones
                    .Where(m => m.PlannedDate <= currentDate && !m.IsDeleted);
                cumulativePlanned = plannedMilestones.Sum(m => m.PaymentAmount ?? 0);

                plannedSeries.DataPoints.Add(new SCurveDataPointDto
                {
                    Date = currentDate,
                    CumulativeValue = cumulativePlanned
                });

                // Actual, invoiced and paid from valuations
                if (currentDate <= DateTime.UtcNow)
                {
                    var valuations = contract.Valuations
                        .Where(v => v.PreparedDate <= currentDate && !v.IsDeleted);
                    
                    cumulativeActual = valuations.Sum(v => v.GrossValuation);
                    cumulativeInvoiced = valuations.Where(v => v.IsInvoiced).Sum(v => v.AmountDue);
                    cumulativePaid = valuations.Where(v => v.IsPaid).Sum(v => v.PaymentAmount);

                    actualSeries.DataPoints.Add(new SCurveDataPointDto
                    {
                        Date = currentDate,
                        CumulativeValue = cumulativeActual
                    });

                    invoicedSeries.DataPoints.Add(new SCurveDataPointDto
                    {
                        Date = currentDate,
                        CumulativeValue = cumulativeInvoiced
                    });

                    paidSeries.DataPoints.Add(new SCurveDataPointDto
                    {
                        Date = currentDate,
                        CumulativeValue = cumulativePaid
                    });
                }

                currentDate = currentDate.AddMonths(1);
            }

            sCurve.Series.Add(plannedSeries);
            sCurve.Series.Add(actualSeries);
            sCurve.Series.Add(invoicedSeries);
            sCurve.Series.Add(paidSeries);

            return sCurve;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate contract S-curve");
            throw;
        }
    }

    #endregion

    #region Export Methods

    public async Task<Dictionary<string, object>> GenerateDashboardChartsAsync(Guid projectId)
    {
        try
        {
            var dashboardData = new Dictionary<string, object>();

            // Generate key charts
            var costSCurve = await GenerateCostSCurveAsync(projectId);
            var progressSCurve = await GenerateProgressSCurveAsync(projectId);
            var evmSCurve = await GenerateEarnedValueSCurveAsync(projectId);

            dashboardData["costSCurve"] = costSCurve;
            dashboardData["progressSCurve"] = progressSCurve;
            dashboardData["evmSCurve"] = evmSCurve;

            // Add summary metrics
            var evmData = await _evmService.CalculateEVMAsync(projectId);
            dashboardData["evmMetrics"] = new
            {
                evmData.CPI,
                evmData.SPI,
                evmData.CostVariance,
                evmData.ScheduleVariance,
                evmData.EAC,
                evmData.ETC,
                evmData.VAC,
                evmData.TCPI
            };

            return dashboardData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate dashboard charts");
            throw;
        }
    }

    public async Task<byte[]> ExportGanttToPdfAsync(Guid projectId, GanttConfigDto? config = null)
    {
        try
        {
            // This would use a PDF generation library
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export Gantt to PDF");
            throw;
        }
    }

    public async Task<byte[]> ExportSCurveToPdfAsync(Guid projectId, string curveType, SCurveConfigDto? config = null)
    {
        try
        {
            // This would use a PDF generation library
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export S-curve to PDF");
            throw;
        }
    }

    public async Task<byte[]> ExportGanttDataToExcelAsync(Guid projectId)
    {
        try
        {
            // This would use IExcelExportService
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export Gantt data to Excel");
            throw;
        }
    }

    public async Task<byte[]> ExportSCurveDataToExcelAsync(Guid projectId, string curveType)
    {
        try
        {
            // This would use IExcelExportService
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export S-curve data to Excel");
            throw;
        }
    }

    #endregion
}

// Extension method for date calculations
public static class DateTimeExtensions
{
    public static DateTime AddWeeks(this DateTime date, int weeks)
    {
        return date.AddDays(weeks * 7);
    }
}