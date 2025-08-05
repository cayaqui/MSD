using Core.Enums.Projects;

namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for project dashboard data
/// </summary>
public class ProjectDashboardDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
    
    // Financial Summary
    public decimal TotalBudget { get; set; }
    public decimal? ActualCost { get; set; }
    public decimal? CommittedCost { get; set; }
    public decimal BudgetVariance { get; set; }
    public decimal BudgetVariancePercentage { get; set; }
    
    // Schedule Summary
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public int PlannedDuration { get; set; }
    public int? ActualDuration { get; set; }
    public int? DaysOverdue { get; set; }
    
    // Progress Summary
    public decimal ProgressPercentage { get; set; }
    public decimal? PlannedProgress { get; set; }
    public decimal ScheduleVariance { get; set; }
    
    // Team Summary
    public int TotalTeamMembers { get; set; }
    public int ActiveTeamMembers { get; set; }
    
    // Risk Summary
    public int HighRisks { get; set; }
    public int MediumRisks { get; set; }
    public int LowRisks { get; set; }
    
    // Change Summary
    public int? ChangeOrderCount { get; set; }
    public decimal? ApprovedChangeValue { get; set; }
    
    // Key Metrics
    public decimal? CPI { get; set; } // Cost Performance Index
    public decimal? SPI { get; set; } // Schedule Performance Index
    public decimal? EAC { get; set; } // Estimate at Completion
    public decimal? VAC { get; set; } // Variance at Completion
}