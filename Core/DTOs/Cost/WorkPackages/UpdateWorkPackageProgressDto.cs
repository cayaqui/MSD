namespace Core.DTOs.Cost.WorkPackages;

public class UpdateWorkPackageProgressDto
{
    public decimal ProgressPercentage { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string? ProgressNotes { get; set; }
}