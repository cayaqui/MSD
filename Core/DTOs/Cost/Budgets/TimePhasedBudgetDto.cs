namespace Core.DTOs.Cost.Budgets;

/// <summary>
/// DTO for time-phased budget data with EVM metrics
/// </summary>
public class TimePhasedBudgetDto
{
    /// <summary>
    /// The period date for this data point
    /// </summary>
    public DateTime Period { get; set; }
    
    /// <summary>
    /// Display name for the period (e.g., "Jan 2024", "Q1 2024")
    /// </summary>
    public string PeriodName { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of period: Daily, Weekly, Monthly, Quarterly
    /// </summary>
    public string PeriodType { get; set; } = "Monthly";
    
    // Budget Values
    /// <summary>
    /// Budgeted amount for this period
    /// </summary>
    public decimal BudgetAmount { get; set; }
    
    /// <summary>
    /// Cumulative budget up to this period
    /// </summary>
    public decimal CumulativeBudget { get; set; }
    
    // Actual Values
    /// <summary>
    /// Actual cost for this period
    /// </summary>
    public decimal? ActualAmount { get; set; }
    
    /// <summary>
    /// Cumulative actual cost up to this period
    /// </summary>
    public decimal? CumulativeActual { get; set; }
    
    // EVM Values (optional - used when integrated with EVM)
    /// <summary>
    /// Planned Value (PV/BCWS) for this period
    /// </summary>
    public decimal? PlannedValue { get; set; }
    
    /// <summary>
    /// Cumulative Planned Value up to this period
    /// </summary>
    public decimal? PlannedValueCumulative { get; set; }
    
    /// <summary>
    /// Earned Value (EV/BCWP) for this period
    /// </summary>
    public decimal? EarnedValue { get; set; }
    
    /// <summary>
    /// Cumulative Earned Value up to this period
    /// </summary>
    public decimal? EarnedValueCumulative { get; set; }
    
    // Variances
    /// <summary>
    /// Cost variance for this period (Budget - Actual)
    /// </summary>
    public decimal? Variance { get; set; }
    
    /// <summary>
    /// Variance percentage ((Budget - Actual) / Budget * 100)
    /// </summary>
    public decimal? VariancePercentage { get; set; }
    
    /// <summary>
    /// Schedule variance when using EVM (EV - PV)
    /// </summary>
    public decimal? ScheduleVariance { get; set; }
    
    /// <summary>
    /// Cost variance when using EVM (EV - AC)
    /// </summary>
    public decimal? CostVariance { get; set; }
}