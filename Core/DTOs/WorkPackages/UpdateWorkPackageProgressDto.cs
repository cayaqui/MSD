namespace Core.DTOs.WorkPackages;

/// <summary>
/// DTO for updating Work Package progress
/// </summary>
public class UpdateWorkPackageProgressDto
{
    public decimal ProgressPercentage { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public string? Comments { get; set; }
    public DateTime ProgressDate { get; set; }
}
