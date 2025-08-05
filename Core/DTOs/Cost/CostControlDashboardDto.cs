
using Core.DTOs.Cost.CostControlReports;

namespace Core.DTOs.Cost;

/// <summary>
/// DTO for cost control dashboard data
/// </summary>
public class CostControlDashboardDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    
    // Budget Summary
    public decimal TotalBudget { get; set; }
    public decimal ApprovedBudget { get; set; }
    public decimal ContingencyAmount { get; set; }
    public decimal ManagementReserve { get; set; }
    
    // Cost Summary
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public decimal CostVariance { get; set; }
    public decimal CostVariancePercentage { get; set; }
    
    // EVM Metrics
    public decimal PlannedValue { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
    public decimal EstimateAtCompletion { get; set; }
    public decimal EstimateToComplete { get; set; }
    public decimal VarianceAtCompletion { get; set; }
    
    // Progress
    public decimal OverallProgress { get; set; }
    public decimal PhysicalProgress { get; set; }
    public decimal FinancialProgress { get; set; }
    
    // Key Indicators
    public int ActiveCommitments { get; set; }
    public int PendingInvoices { get; set; }
    public int OverduePayments { get; set; }
    public decimal UncommittedBalance { get; set; }
    
    // Trends
    public List<CostTrendDto> CostTrends { get; set; } = new();
    public List<CostSummaryByCategoryDto> CostByCategory { get; set; } = new();
    public List<CostSummaryByControlAccountDto> CostByControlAccount { get; set; } = new();
    
    // Last Update Info
    public DateTime? LastUpdated { get; set; }
    public string? LastUpdatedBy { get; set; }
    
    // Currency
    public string Currency { get; set; } = "USD";
}