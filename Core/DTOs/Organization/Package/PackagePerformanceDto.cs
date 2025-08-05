namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for package performance metrics
/// </summary>
public class PackagePerformanceDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    // Schedule Performance
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int PlannedDuration { get; set; }
    public int? ActualDuration { get; set; }
    public decimal ScheduleVariance { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
    public bool IsOnSchedule { get; set; }
    public int? DaysAheadBehind { get; set; }
    
    // Cost Performance
    public decimal ContractValue { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CostVariance { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public bool IsOnBudget { get; set; }
    
    // Progress Performance
    public decimal ProgressPercentage { get; set; }
    public decimal PlannedProgress { get; set; }
    public decimal ProgressVariance { get; set; }
    
    // Quality Metrics
    public int TotalIssues { get; set; }
    public int OpenIssues { get; set; }
    public int ClosedIssues { get; set; }
    public decimal IssueClosureRate { get; set; }
    
    // Resource Performance
    public decimal PlannedManHours { get; set; }
    public decimal ActualManHours { get; set; }
    public decimal ProductivityIndex { get; set; }
}