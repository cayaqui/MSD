namespace Core.DTOs.Reports;

public class EarnedValueTrendDto
{
    public DateTime Date { get; set; }
    public decimal PlannedValue { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CostVariance { get; set; }
    public decimal ScheduleVariance { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
}