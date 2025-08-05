namespace Core.DTOs.Risk.Risk;

public class RiskTrendDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string PeriodLabel { get; set; } = string.Empty;
    
    // Risk Counts
    public int NewRisks { get; set; }
    public int ClosedRisks { get; set; }
    public int TotalActiveRisks { get; set; }
    
    // Risk Scores
    public decimal AverageRiskScore { get; set; }
    public decimal MaxRiskScore { get; set; }
    public int CriticalCount { get; set; }
    public int HighCount { get; set; }
    public int MediumCount { get; set; }
    public int LowCount { get; set; }
    
    // Response Metrics
    public int ResponsesInitiated { get; set; }
    public int ResponsesCompleted { get; set; }
    public decimal ResponseCompletionRate { get; set; }
    
    // Impact Metrics
    public decimal TotalCostExposure { get; set; }
    public decimal MitigatedCostExposure { get; set; }
    public int TotalScheduleExposure { get; set; }
    public int MitigatedScheduleExposure { get; set; }
}