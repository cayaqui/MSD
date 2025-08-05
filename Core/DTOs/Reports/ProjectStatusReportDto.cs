namespace Core.DTOs.Reports;

public class ProjectStatusReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public string ReportPeriod { get; set; } = string.Empty;
    
    // Project Overview
    public string ProjectStatus { get; set; } = string.Empty;
    public DateTime ProjectStartDate { get; set; }
    public DateTime ProjectEndDate { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal PercentageComplete { get; set; }
    
    // Schedule Information
    public int TotalActivities { get; set; }
    public int CompletedActivities { get; set; }
    public int InProgressActivities { get; set; }
    public int DelayedActivities { get; set; }
    public decimal ScheduleVariance { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
    
    // Cost Information
    public decimal BudgetAtCompletion { get; set; }
    public decimal ActualCost { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal PlannedValue { get; set; }
    public decimal CostVariance { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal EstimateAtCompletion { get; set; }
    public decimal EstimateToComplete { get; set; }
    
    // Key Milestones
    public List<MilestoneSummaryDto> KeyMilestones { get; set; } = new();
    
    // Issues and Risks
    public int OpenIssues { get; set; }
    public int HighRisks { get; set; }
    public int MediumRisks { get; set; }
    public int LowRisks { get; set; }
    
    // Summary Sections
    public string ExecutiveSummary { get; set; } = string.Empty;
    public string AccomplishmentsThisPeriod { get; set; } = string.Empty;
    public string PlannedActivitiesNextPeriod { get; set; } = string.Empty;
    public string KeyIssuesAndConcerns { get; set; } = string.Empty;
    public string RecommendationsAndActions { get; set; } = string.Empty;
    
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class MilestoneSummaryDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = string.Empty; // OnTrack, Delayed, Completed, AtRisk
    public int DaysVariance { get; set; }
    public string? Notes { get; set; }
}