namespace Core.DTOs.Reports;

public class ScheduleProgressReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime AsOfDate { get; set; }
    
    // Overall Schedule Metrics
    public DateTime ProjectStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime ForecastEndDate { get; set; }
    public int TotalDuration { get; set; } // in days
    public int ElapsedDuration { get; set; } // in days
    public int RemainingDuration { get; set; } // in days
    public decimal OverallProgress { get; set; } // percentage
    
    // Activity Statistics
    public int TotalActivities { get; set; }
    public int NotStartedActivities { get; set; }
    public int InProgressActivities { get; set; }
    public int CompletedActivities { get; set; }
    public int DelayedActivities { get; set; }
    public int CriticalActivities { get; set; }
    
    // Schedule Performance
    public decimal SchedulePerformanceIndex { get; set; } // SPI
    public decimal ScheduleVariance { get; set; } // SV in days
    public decimal CriticalPathVariance { get; set; } // in days
    public decimal TotalFloat { get; set; } // in days
    
    // Critical Path Information
    public List<CriticalPathActivityDto> CriticalPath { get; set; } = new();
    public int CriticalPathLength { get; set; } // in days
    public DateTime CriticalPathEndDate { get; set; }
    
    // Near Critical Paths
    public List<NearCriticalPathDto> NearCriticalPaths { get; set; } = new();
    
    // Milestone Status
    public List<MilestoneStatusDto> Milestones { get; set; } = new();
    
    // Activity Details by Status
    public List<ActivityProgressDto> UpcomingActivities { get; set; } = new();
    public List<ActivityProgressDto> ActiveActivities { get; set; } = new();
    public List<ActivityProgressDto> DelayedActivitiesList { get; set; } = new();
    public List<ActivityProgressDto> RecentlyCompletedActivities { get; set; } = new();
    
    // Resource Loading
    public decimal PeakResourceDemand { get; set; }
    public DateTime PeakResourceDate { get; set; }
    public decimal CurrentResourceUtilization { get; set; }
    
    // Schedule Health Indicators
    public string ScheduleHealth { get; set; } = string.Empty; // Good, AtRisk, Critical
    public List<ScheduleRiskDto> ScheduleRisks { get; set; } = new();
    
    // Lookahead Schedule
    public int LookaheadPeriod { get; set; } // in days
    public List<LookaheadActivityDto> LookaheadActivities { get; set; } = new();
    
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class CriticalPathActivityDto
{
    public string ActivityId { get; set; } = string.Empty;
    public string ActivityName { get; set; } = string.Empty;
    public DateTime EarlyStart { get; set; }
    public DateTime EarlyFinish { get; set; }
    public DateTime LateStart { get; set; }
    public DateTime LateFinish { get; set; }
    public int Duration { get; set; }
    public decimal PercentageComplete { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class NearCriticalPathDto
{
    public string PathName { get; set; } = string.Empty;
    public int PathLength { get; set; } // in days
    public decimal TotalFloat { get; set; } // in days
    public int ActivityCount { get; set; }
    public string CriticalityLevel { get; set; } = string.Empty; // High, Medium, Low
}


public class ActivityProgressDto
{
    public string ActivityId { get; set; } = string.Empty;
    public string ActivityName { get; set; } = string.Empty;
    public DateTime PlannedStart { get; set; }
    public DateTime PlannedFinish { get; set; }
    public DateTime? ActualStart { get; set; }
    public DateTime? ActualFinish { get; set; }
    public DateTime? ForecastFinish { get; set; }
    public int Duration { get; set; }
    public decimal PercentageComplete { get; set; }
    public decimal Float { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ResponsibleParty { get; set; } = string.Empty;
    public List<string> Predecessors { get; set; } = new();
    public List<string> Successors { get; set; } = new();
}

public class ScheduleRiskDto
{
    public string RiskType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty; // High, Medium, Low
    public string Probability { get; set; } = string.Empty; // High, Medium, Low
    public string MitigationStrategy { get; set; } = string.Empty;
    public List<string> AffectedActivities { get; set; } = new();
}

public class LookaheadActivityDto
{
    public string ActivityId { get; set; } = string.Empty;
    public string ActivityName { get; set; } = string.Empty;
    public DateTime PlannedStart { get; set; }
    public DateTime PlannedFinish { get; set; }
    public string ResponsibleParty { get; set; } = string.Empty;
    public string ConstraintStatus { get; set; } = string.Empty; // Clear, Pending, Blocked
    public List<string> Prerequisites { get; set; } = new();
    public string ReadinessStatus { get; set; } = string.Empty;
}