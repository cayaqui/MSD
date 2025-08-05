namespace Core.DTOs.Risk.Risk;

public class RiskDashboardDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    
    // Overview
    public int TotalRisks { get; set; }
    public int ActiveRisks { get; set; }
    public int ClosedRisks { get; set; }
    public int RisksRequiringReview { get; set; }
    public int OverdueResponses { get; set; }
    
    // Risk Levels
    public int CriticalRisks { get; set; }
    public int HighRisks { get; set; }
    public int MediumRisks { get; set; }
    public int LowRisks { get; set; }
    
    // Risk Types
    public int Threats { get; set; }
    public int Opportunities { get; set; }
    
    // Financial Impact
    public decimal TotalCostExposure { get; set; }
    public decimal MitigatedCostExposure { get; set; }
    public decimal ResidualCostExposure { get; set; }
    
    // Schedule Impact
    public int TotalScheduleExposure { get; set; }
    public int MitigatedScheduleExposure { get; set; }
    public int ResidualScheduleExposure { get; set; }
    
    // Response Status
    public int ResponsesPlanned { get; set; }
    public int ResponsesInProgress { get; set; }
    public int ResponsesCompleted { get; set; }
    public decimal ResponseEffectiveness { get; set; }
    
    // Top Risks
    public List<RiskSummaryDto> TopRisksByScore { get; set; } = new();
    public List<RiskSummaryDto> TopRisksByCost { get; set; } = new();
    public List<RiskSummaryDto> TopRisksBySchedule { get; set; } = new();
    
    // Trends
    public List<RiskTrendPoint> RiskScoreTrend { get; set; } = new();
    public List<CategoryDistribution> RisksByCategory { get; set; } = new();
}

public class RiskSummaryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public decimal? CostImpact { get; set; }
    public int? ScheduleImpact { get; set; }
    public string ResponseOwner { get; set; } = string.Empty;

}

public class RiskTrendPoint
{
    public DateTime Date { get; set; }
    public int TotalRisks { get; set; }
    public int ActiveRisks { get; set; }
    public decimal AverageRiskScore { get; set; }
}

public class CategoryDistribution
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}