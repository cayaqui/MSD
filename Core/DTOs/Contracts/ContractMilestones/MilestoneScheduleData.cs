using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.ContractMilestones;

public class MilestoneScheduleData
{
    public Guid MilestoneId { get; set; }
    public string MilestoneCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public DateTime? ForecastDate { get; set; }
    public int? VarianceDays { get; set; }
    public MilestoneStatus Status { get; set; }
    public bool IsCriticalPath { get; set; }
}