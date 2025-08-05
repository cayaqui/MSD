namespace Core.DTOs.Contracts.ContractMilestones;

public class UpdateMilestoneProgressDto
{
    public decimal PercentageComplete { get; set; }
    public string ProgressComments { get; set; } = string.Empty;
    public DateTime? ForecastDate { get; set; }
}
