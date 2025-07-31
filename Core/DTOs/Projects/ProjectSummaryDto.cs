namespace Core.DTOs.Projects;

/// <summary>
/// DTO for project summary with additional calculated fields
/// </summary>
public class ProjectSummaryDto : ProjectDto
{
    // Budget metrics
    public decimal BudgetUtilization { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }

    // Schedule metrics
    public int DaysRemaining { get; set; }
    public int DaysOverdue { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsOverBudget { get; set; }

    // Team metrics
    public int ActiveTeamMembers { get; set; }
    public decimal TeamUtilization { get; set; }

    // Progress metrics
    public int CompletedPhases { get; set; }
    public int TotalPhases { get; set; }
    public decimal PhaseProgress { get; set; }
}
