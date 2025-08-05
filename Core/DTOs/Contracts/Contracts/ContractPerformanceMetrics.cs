namespace Core.DTOs.Contracts.Contracts;

public class ContractPerformanceMetrics
{
    public Guid ContractId { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public int ScheduleVariance { get; set; }
    public decimal CostVariance { get; set; }
    public int TimeElapsed { get; set; }
    public int TimeRemaining { get; set; }
    public int MilestonesCompleted { get; set; }
    public int MilestonesTotal { get; set; }
    public bool IsDelayed { get; set; }
    public bool HasExpiredBonds { get; set; }
}