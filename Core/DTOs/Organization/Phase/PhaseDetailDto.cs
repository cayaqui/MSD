namespace Core.DTOs.Organization.Phase;

/// <summary>
/// DTO for detailed phase information including related data
/// </summary>
public class PhaseDetailDto : PhaseDto
{
    // Related Entities
    public int WBSElementCount { get; set; }
    public int ControlAccountCount { get; set; }
    public int MilestoneCount { get; set; }
    public int CompletedMilestones { get; set; }
    public int PlanningPackageCount { get; set; }
    
    // Financial Details
    public decimal BudgetVariance { get; set; }
    public decimal BudgetVariancePercentage { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    
    // Schedule Details
    public int PlannedDuration { get; set; }
    public int? ActualDuration { get; set; }
    public int? DaysRemaining { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
    
    // Resource Summary
    public int AssignedResources { get; set; }
    public decimal PlannedEffort { get; set; }
    public decimal ActualEffort { get; set; }
}