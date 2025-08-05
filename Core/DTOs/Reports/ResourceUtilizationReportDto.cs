namespace Core.DTOs.Reports;

public class ResourceUtilizationReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    // Overall Resource Metrics
    public int TotalResources { get; set; }
    public int ActiveResources { get; set; }
    public decimal OverallUtilizationRate { get; set; } // percentage
    public decimal AverageUtilizationRate { get; set; }
    public decimal PeakUtilizationRate { get; set; }
    public DateTime PeakUtilizationDate { get; set; }
    
    // Resource Categories
    public List<ResourceCategoryDto> ResourceCategories { get; set; } = new();
    
    // Individual Resource Utilization
    public List<ResourceUtilizationDetailDto> Resources { get; set; } = new();
    
    // Team/Department Utilization
    public List<TeamUtilizationDto> Teams { get; set; } = new();
    
    // Resource Allocation vs Actual
    public ResourceAllocationDto Allocation { get; set; } = new();
    
    // Cost Analysis
    public ResourceCostAnalysisDto CostAnalysis { get; set; } = new();
    
    // Resource Demand Forecast
    public ResourceDemandForecastDto Forecast { get; set; } = new();
    
    // Overtime Analysis
    public OvertimeAnalysisDto OvertimeAnalysis { get; set; } = new();
    
    // Resource Conflicts
    public List<ResourceConflictDto> Conflicts { get; set; } = new();
    
    // Time-Phased Utilization
    public List<TimePhasedUtilizationDto> TimePhasedData { get; set; } = new();
    
    // Key Performance Indicators
    public ResourceKPIDto KPIs { get; set; } = new();
    
    // Executive Summary
    public string ExecutiveSummary { get; set; } = string.Empty;
    public List<string> KeyFindings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class ResourceCategoryDto
{
    public string CategoryName { get; set; } = string.Empty; // Labor, Equipment, Material
    public int ResourceCount { get; set; }
    public decimal UtilizationRate { get; set; }
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal AvailableHours { get; set; }
    public decimal Cost { get; set; }
    public List<ResourceTypeDto> ResourceTypes { get; set; } = new();
}

public class ResourceTypeDto
{
    public string TypeName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal UtilizationRate { get; set; }
    public decimal AverageCostPerHour { get; set; }
}

public class ResourceUtilizationDetailDto
{
    public string ResourceId { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    
    // Utilization Metrics
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal AvailableHours { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public decimal ProductiveHours { get; set; }
    public decimal NonProductiveHours { get; set; }
    public decimal OvertimeHours { get; set; }
    
    // Cost Metrics
    public decimal StandardRate { get; set; }
    public decimal OvertimeRate { get; set; }
    public decimal TotalCost { get; set; }
    
    // Assignment Details
    public List<ResourceAssignmentDto> Assignments { get; set; } = new();
    
    // Availability
    public List<ResourceAvailabilityDto> Availability { get; set; } = new();
}

public class ResourceAssignmentDto
{
    public string TaskId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal CompletionPercentage { get; set; }
}

public class ResourceAvailabilityDto
{
    public DateTime Date { get; set; }
    public decimal AvailableHours { get; set; }
    public decimal AssignedHours { get; set; }
    public decimal RemainingCapacity { get; set; }
    public string Status { get; set; } = string.Empty; // Available, PartiallyAvailable, FullyAllocated, Overallocated
}

public class TeamUtilizationDto
{
    public string TeamName { get; set; } = string.Empty;
    public string Manager { get; set; } = string.Empty;
    public int TeamSize { get; set; }
    public decimal TeamUtilizationRate { get; set; }
    public decimal TotalPlannedHours { get; set; }
    public decimal TotalActualHours { get; set; }
    public decimal TotalAvailableHours { get; set; }
    public decimal TeamEfficiency { get; set; }
    public List<string> TopContributors { get; set; } = new();
    public List<string> UnderutilizedMembers { get; set; } = new();
}

public class ResourceAllocationDto
{
    public decimal TotalAllocatedHours { get; set; }
    public decimal TotalAvailableHours { get; set; }
    public decimal AllocationPercentage { get; set; }
    public int OverallocatedResources { get; set; }
    public int UnderallocatedResources { get; set; }
    public int OptimallyAllocatedResources { get; set; }
    public List<AllocationVarianceDto> Variances { get; set; } = new();
}

public class AllocationVarianceDto
{
    public string ResourceName { get; set; } = string.Empty;
    public decimal PlannedAllocation { get; set; }
    public decimal ActualAllocation { get; set; }
    public decimal Variance { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ResourceCostAnalysisDto
{
    public decimal BudgetedLaborCost { get; set; }
    public decimal ActualLaborCost { get; set; }
    public decimal LaborCostVariance { get; set; }
    public decimal RegularTimeCost { get; set; }
    public decimal OvertimeCost { get; set; }
    public decimal CostPerProductiveHour { get; set; }
    public decimal ResourceEfficiencyRatio { get; set; }
    public List<CostByResourceTypeDto> CostByType { get; set; } = new();
}

public class CostByResourceTypeDto
{
    public string ResourceType { get; set; } = string.Empty;
    public decimal BudgetedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal Variance { get; set; }
    public decimal PercentageOfTotal { get; set; }
}

public class ResourceDemandForecastDto
{
    public List<ForecastPeriodDto> ForecastPeriods { get; set; } = new();
    public List<SkillGapDto> IdentifiedSkillGaps { get; set; } = new();
    public decimal PeakDemand { get; set; }
    public DateTime PeakDemandDate { get; set; }
    public List<string> RecommendedActions { get; set; } = new();
}

public class ForecastPeriodDto
{
    public DateTime Period { get; set; }
    public decimal RequiredHours { get; set; }
    public decimal AvailableHours { get; set; }
    public decimal Gap { get; set; }
    public List<RequiredSkillDto> RequiredSkills { get; set; } = new();
}

public class RequiredSkillDto
{
    public string SkillName { get; set; } = string.Empty;
    public int RequiredCount { get; set; }
    public int AvailableCount { get; set; }
    public int Gap { get; set; }
}

public class SkillGapDto
{
    public string Skill { get; set; } = string.Empty;
    public int CurrentCapacity { get; set; }
    public int RequiredCapacity { get; set; }
    public int Gap { get; set; }
    public string Priority { get; set; } = string.Empty; // Critical, High, Medium, Low
    public string RecommendedAction { get; set; } = string.Empty;
}

public class OvertimeAnalysisDto
{
    public decimal TotalOvertimeHours { get; set; }
    public decimal OvertimeCost { get; set; }
    public decimal OvertimePercentage { get; set; }
    public int ResourcesWithOvertime { get; set; }
    public List<OvertimeByResourceDto> TopOvertimeResources { get; set; } = new();
    public List<OvertimeTrendDto> OvertimeTrend { get; set; } = new();
}

public class OvertimeByResourceDto
{
    public string ResourceName { get; set; } = string.Empty;
    public decimal OvertimeHours { get; set; }
    public decimal OvertimeCost { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class OvertimeTrendDto
{
    public DateTime Week { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal OvertimeCost { get; set; }
    public int ResourceCount { get; set; }
}

public class ResourceConflictDto
{
    public string ConflictType { get; set; } = string.Empty; // Overallocation, SkillMismatch, Availability
    public string ResourceName { get; set; } = string.Empty;
    public List<string> ConflictingTasks { get; set; } = new();
    public DateTime ConflictDate { get; set; }
    public string Severity { get; set; } = string.Empty; // High, Medium, Low
    public string ProposedResolution { get; set; } = string.Empty;
}

public class TimePhasedUtilizationDto
{
    public DateTime Period { get; set; }
    public decimal UtilizationRate { get; set; }
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal AvailableHours { get; set; }
    public int ActiveResources { get; set; }
    public decimal Cost { get; set; }
}

public class ResourceKPIDto
{
    public decimal ResourceProductivity { get; set; } // Output per hour
    public decimal ResourceEfficiency { get; set; } // Productive hours / Total hours
    public decimal CapacityUtilization { get; set; } // Used capacity / Available capacity
    public decimal ResourceAvailability { get; set; } // Available hours / Total hours
    public decimal OvertimeRatio { get; set; } // Overtime / Regular hours
    public decimal CostEfficiencyIndex { get; set; } // Budgeted cost / Actual cost
    public decimal ScheduleAdherence { get; set; } // On-time task completion rate
    public decimal SkillMatchIndex { get; set; } // Right skills for tasks
}