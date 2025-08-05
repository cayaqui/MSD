namespace Core.DTOs.Cost;

public class GenerateCostControlReportDto
{
    public Guid ProjectId { get; set; }
    public DateTime ReportDate { get; set; }
    public string PeriodType { get; set; } = string.Empty; // Weekly, Monthly, Quarterly, Annual
    public int PeriodNumber { get; set; }
    public bool IncludeForecasts { get; set; }
    public bool IncludeControlAccounts { get; set; }
    public string? Notes { get; set; }
}