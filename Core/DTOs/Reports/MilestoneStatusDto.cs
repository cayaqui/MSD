namespace Core.DTOs.Reports;

public class MilestoneStatusDto
{
    public Guid MilestoneId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime PlannedDate { get; set; }
    public DateTime? ForecastDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Progress { get; set; }
    public bool IsCritical { get; set; }
    public bool IsContractual { get; set; }
    public int VarianceDays { get; set; }
}