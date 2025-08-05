namespace Core.DTOs.Reports;

public class MilestoneReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    
    // Project Overview
    public DateTime ProjectStartDate { get; set; }
    public DateTime ProjectEndDate { get; set; }
    public int TotalMilestones { get; set; }
    public int CompletedMilestones { get; set; }
    public int UpcomingMilestones { get; set; }
    public int DelayedMilestones { get; set; }
    public int AtRiskMilestones { get; set; }
    
    // Overall Performance
    public decimal MilestoneCompletionRate { get; set; } // percentage
    public decimal OnTimeDeliveryRate { get; set; } // percentage
    public decimal AverageDelayDays { get; set; }
    
    // Milestone Categories
    public List<MilestoneCategoryDto> Categories { get; set; } = new();
    
    // Detailed Milestone List
    public List<MilestoneDetailDto> Milestones { get; set; } = new();
    
    // Timeline View Data
    public List<MilestoneTimelineDto> TimelineData { get; set; } = new();
    
    // Milestone Dependencies
    public List<MilestoneDependencyDto> Dependencies { get; set; } = new();
    
    // Trend Analysis
    public List<MilestoneTrendDto> TrendData { get; set; } = new();
    
    // Executive Summary
    public string ExecutiveSummary { get; set; } = string.Empty;
    public List<string> KeyAchievements { get; set; } = new();
    public List<string> CriticalIssues { get; set; } = new();
    public List<string> RecommendedActions { get; set; } = new();
    
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class MilestoneCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int CompletedCount { get; set; }
    public int OnTrackCount { get; set; }
    public int AtRiskCount { get; set; }
    public int DelayedCount { get; set; }
    public decimal CompletionPercentage { get; set; }
}

public class MilestoneDetailDto
{
    public string MilestoneId { get; set; } = string.Empty;
    public string MilestoneName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Start, Finish, Intermediate, Payment, Regulatory
    public DateTime PlannedDate { get; set; }
    public DateTime? BaselineDate { get; set; }
    public DateTime? ForecastDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = string.Empty; // NotStarted, InProgress, Completed, Delayed, AtRisk
    public int VarianceDays { get; set; }
    public string Owner { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty; // High, Medium, Low
    public decimal? PaymentAmount { get; set; }
    public string? PaymentStatus { get; set; }
    public List<string> Deliverables { get; set; } = new();
    public List<string> Predecessors { get; set; } = new();
    public List<string> Successors { get; set; } = new();
    public string? Notes { get; set; }
    public List<MilestoneRiskDto> Risks { get; set; } = new();
}

public class MilestoneTimelineDto
{
    public string Quarter { get; set; } = string.Empty;
    public int Year { get; set; }
    public List<TimelineMilestoneDto> Milestones { get; set; } = new();
}

public class TimelineMilestoneDto
{
    public string MilestoneId { get; set; } = string.Empty;
    public string MilestoneName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class MilestoneDependencyDto
{
    public string FromMilestoneId { get; set; } = string.Empty;
    public string FromMilestoneName { get; set; } = string.Empty;
    public string ToMilestoneId { get; set; } = string.Empty;
    public string ToMilestoneName { get; set; } = string.Empty;
    public string DependencyType { get; set; } = string.Empty; // FS, SS, FF, SF
    public int LagDays { get; set; }
    public string Status { get; set; } = string.Empty; // Active, Satisfied, Violated
}

public class MilestoneTrendDto
{
    public DateTime Period { get; set; }
    public int PlannedCompletions { get; set; }
    public int ActualCompletions { get; set; }
    public decimal OnTimeDeliveryRate { get; set; }
    public decimal AverageDelay { get; set; }
}

public class MilestoneRiskDto
{
    public string RiskDescription { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty; // High, Medium, Low
    public string Probability { get; set; } = string.Empty; // High, Medium, Low
    public string MitigationPlan { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
}