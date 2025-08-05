namespace Core.DTOs.Contracts.ContractMilestones;

public class MilestoneScheduleVariance
{
    public Guid MilestoneId { get; set; }
    public DateTime PlannedDate { get; set; }
    public DateTime? ForecastDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public int VarianceDays { get; set; }
    public bool IsDelayed { get; set; }
    public string? DelayReason { get; set; }
}