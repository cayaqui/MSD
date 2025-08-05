namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for updating package dates
/// </summary>
public class UpdatePackageDatesDto
{
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
}