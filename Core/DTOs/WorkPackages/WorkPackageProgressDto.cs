namespace Core.DTOs.WorkPackages;

/// <summary>
/// DTO for Work Package progress record
/// </summary>
public class WorkPackageProgressDto
{
    public Guid Id { get; set; }
    public Guid WorkPackageId { get; set; }
    public DateTime ProgressDate { get; set; }
    public decimal ProgressPercentage { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public string? Comments { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
