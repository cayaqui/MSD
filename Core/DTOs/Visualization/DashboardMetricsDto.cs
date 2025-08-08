namespace Core.DTOs.Visualization;

public class DashboardMetricsDto
{
    public ProjectMetricsDto ProjectMetrics { get; set; } = new();
    public ScheduleMetricsDto ScheduleMetrics { get; set; } = new();
    public CostMetricsDto CostMetrics { get; set; } = new();
    public QualityMetricsDto QualityMetrics { get; set; } = new();
    public RiskMetricsDto RiskMetrics { get; set; } = new();
    public ContractMetricsDto ContractMetrics { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class ProjectMetricsDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public decimal OverallProgress { get; set; }
    public string HealthStatus { get; set; } = string.Empty;
    public int DaysElapsed { get; set; }
    public int DaysRemaining { get; set; }
}

public class ScheduleMetricsDto
{
    public int TotalActivities { get; set; }
    public int CompletedActivities { get; set; }
    public int InProgressActivities { get; set; }
    public int DelayedActivities { get; set; }
    public int CriticalActivities { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
    public int ScheduleVarianceDays { get; set; }
}

public class CostMetricsDto
{
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal CostVariance { get; set; }
    public decimal BurnRate { get; set; }
}

public class QualityMetricsDto
{
    public int TotalInspections { get; set; }
    public int PassedInspections { get; set; }
    public int FailedInspections { get; set; }
    public int OpenNCRs { get; set; }
    public int ClosedNCRs { get; set; }
    public decimal QualityScore { get; set; }
}

public class RiskMetricsDto
{
    public int TotalRisks { get; set; }
    public int HighRisks { get; set; }
    public int MediumRisks { get; set; }
    public int LowRisks { get; set; }
    public int MitigatedRisks { get; set; }
    public decimal RiskExposure { get; set; }
}

public class ContractMetricsDto
{
    public int TotalContracts { get; set; }
    public int ActiveContracts { get; set; }
    public decimal TotalContractValue { get; set; }
    public decimal InvoicedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public int PendingChangeOrders { get; set; }
    public int OpenClaims { get; set; }
}