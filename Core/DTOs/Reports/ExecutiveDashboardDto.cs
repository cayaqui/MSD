namespace Core.DTOs.Reports;

public class ExecutiveDashboardDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    
    // Project Overview
    public ProjectOverviewDto Overview { get; set; } = new();
    
    // Key Performance Indicators
    public KPISummaryDto KPIs { get; set; } = new();
    
    // Schedule Metrics
    public ScheduleMetricsDto Schedule { get; set; } = new();
    
    // Cost Metrics
    public CostMetricsDto Cost { get; set; } = new();
    
    // Risk Summary
    public RiskSummaryDto Risks { get; set; } = new();
    
    // Quality Metrics
    public QualityMetricsDto Quality { get; set; } = new();
    
    // Recent Activities
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    
    // Upcoming Milestones
    public List<UpcomingMilestoneDto> UpcomingMilestones { get; set; } = new();
}

public class ProjectOverviewDto
{
    public string Description { get; set; } = string.Empty;
    public string Client { get; set; } = string.Empty;
    public string ProjectManager { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ForecastEndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal PercentComplete { get; set; }
    public int DaysRemaining { get; set; }
    public decimal ContractValue { get; set; }
}

public class KPISummaryDto
{
    public decimal OverallHealth { get; set; } // 0-100
    public string HealthStatus { get; set; } = string.Empty; // Good, Warning, Critical
    public decimal SchedulePerformanceIndex { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal QualityIndex { get; set; }
    public decimal SafetyIndex { get; set; }
    public int OpenIssues { get; set; }
    public int OverdueActivities { get; set; }
    public decimal CustomerSatisfaction { get; set; }
}

public class ScheduleMetricsDto
{
    public decimal PercentComplete { get; set; }
    public decimal PlannedProgress { get; set; }
    public decimal ActualProgress { get; set; }
    public decimal ScheduleVariance { get; set; }
    public int TotalActivities { get; set; }
    public int CompletedActivities { get; set; }
    public int InProgressActivities { get; set; }
    public int NotStartedActivities { get; set; }
    public int CriticalPathActivities { get; set; }
    public decimal Float { get; set; }
    public List<WeeklyProgressDto> WeeklyProgress { get; set; } = new();
}

public class CostMetricsDto
{
    public decimal BudgetAtCompletion { get; set; }
    public decimal ActualCost { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal PlannedValue { get; set; }
    public decimal CostVariance { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal EstimateAtCompletion { get; set; }
    public decimal EstimateToComplete { get; set; }
    public decimal VarianceAtCompletion { get; set; }
    public List<MonthlyCostDto> MonthlyCosts { get; set; } = new();
    public List<CostByDisciplineDto> CostByDiscipline { get; set; } = new();
}

public class RiskSummaryDto
{
    public string RiskId { get; set; } = string.Empty;
    public string RiskTitle { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal? FinancialExposure { get; set; }
    public string TrendDirection { get; set; } = string.Empty; // Increasing, Stable, Decreasing
    public int TotalRisks { get; set; }
    public int ActiveRisks { get; set; }
    public int ClosedRisks { get; set; }
    public int CriticalRisks { get; set; }
    public int HighRisks { get; set; }
    public decimal TotalExposure { get; set; }
    public decimal MitigatedExposure { get; set; }
    public List<TopRiskDto> TopRisks { get; set; } = new();
}

public class QualityMetricsDto
{
    public decimal QualityIndex { get; set; }
    public int TotalInspections { get; set; }
    public int PassedInspections { get; set; }
    public int FailedInspections { get; set; }
    public int OpenNCRs { get; set; } // Non-Conformance Reports
    public int ClosedNCRs { get; set; }
    public decimal ReworkPercentage { get; set; }
    public decimal FirstPassYield { get; set; }
}

public class RecentActivityDto
{
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
}

public class UpcomingMilestoneDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime PlannedDate { get; set; }
    public DateTime? ForecastDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int DaysRemaining { get; set; }
    public string Owner { get; set; } = string.Empty;
}

public class WeeklyProgressDto
{
    public int WeekNumber { get; set; }
    public DateTime WeekStartDate { get; set; }
    public decimal PlannedProgress { get; set; }
    public decimal ActualProgress { get; set; }
}

public class MonthlyCostDto
{
    public string Month { get; set; } = string.Empty;
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal EarnedValue { get; set; }
}

public class CostByDisciplineDto
{
    public string Discipline { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal Actual { get; set; }
    public decimal Variance { get; set; }
    public decimal PercentageOfTotal { get; set; }
}

