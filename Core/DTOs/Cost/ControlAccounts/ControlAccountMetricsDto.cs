using Core.DTOs.Common;
using Core.DTOs.Cost.Budgets;

namespace Core.DTOs.Cost.ControlAccounts;

/// <summary>
/// DTO for Control Account EVM Metrics
/// </summary>
public class EVMMetricsDto
{
    public Guid ControlAccountId { get; set; }
    public string ControlAccountCode { get; set; } = string.Empty;
    public string ControlAccountName { get; set; } = string.Empty;
    
    // Basic EVM Metrics
    public decimal PlannedValue { get; set; } // PV (BCWS)
    public decimal EarnedValue { get; set; } // EV (BCWP)
    public decimal ActualCost { get; set; } // AC (ACWP)
    public decimal BudgetAtCompletion { get; set; } // BAC
    
    // Variances
    public decimal CostVariance { get; set; } // CV = EV - AC
    public decimal ScheduleVariance { get; set; } // SV = EV - PV
    public decimal VarianceAtCompletion { get; set; } // VAC = BAC - EAC
    
    // Performance Indices
    public decimal CostPerformanceIndex { get; set; } // CPI = EV / AC
    public decimal SchedulePerformanceIndex { get; set; } // SPI = EV / PV
    public decimal ToCompletePerformanceIndex { get; set; } // TCPI = (BAC - EV) / (BAC - AC)
    
    // Forecasts
    public decimal EstimateAtCompletion { get; set; } // EAC
    public decimal EstimateToComplete { get; set; } // ETC = EAC - AC
    
    // Progress
    public decimal PercentComplete { get; set; }
    public decimal PercentScheduled { get; set; }
    public decimal PercentSpent { get; set; }
    
    // Status
    public string CostStatus { get; set; } = string.Empty; // OnBudget, OverBudget, UnderBudget
    public string ScheduleStatus { get; set; } = string.Empty; // OnSchedule, Behind, Ahead
    public string OverallStatus { get; set; } = string.Empty; // Green, Yellow, Red
    
    public DateTime AsOfDate { get; set; }
    public string Currency { get; set; } = "USD";
}

/// <summary>
/// DTO for Control Account Performance Report
/// </summary>
public class ControlAccountPerformanceDto
{
    public Guid ControlAccountId { get; set; }
    public string ControlAccountCode { get; set; } = string.Empty;
    public string ControlAccountName { get; set; } = string.Empty;
    
    // Summary
    public EVMMetricsDto CurrentMetrics { get; set; } = new();
    public List<TimePhasedBudgetDto> TimePhasedData { get; set; } = new();
    
    // Work Package Summary
    public int TotalWorkPackages { get; set; }
    public int CompletedWorkPackages { get; set; }
    public int ActiveWorkPackages { get; set; }
    public int PlannedWorkPackages { get; set; }
    
    // Planning Package Summary
    public int TotalPlanningPackages { get; set; }
    public decimal PlanningPackageBudget { get; set; }
    
    // Milestone Summary
    public int TotalMilestones { get; set; }
    public int CompletedMilestones { get; set; }
    public int OverdueMilestones { get; set; }
    public DateTime? NextMilestoneDate { get; set; }
    
    // Risk Assessment
    public string RiskLevel { get; set; } = string.Empty; // Low, Medium, High, Critical
    public List<string> RiskFactors { get; set; } = new();
    
    // Trend Analysis
    public decimal CPITrend { get; set; } // Positive = improving, Negative = deteriorating
    public decimal SPITrend { get; set; }
    public int ConsecutivePeriodsOverBudget { get; set; }
    public int ConsecutivePeriodsBehindSchedule { get; set; }
    
    // Forecasts
    public DateTime? ForecastCompletionDate { get; set; }
    public decimal ConfidenceLevel { get; set; } // Percentage confidence in forecasts
    
    public DateTime ReportDate { get; set; }
    public string PreparedBy { get; set; } = string.Empty;
}