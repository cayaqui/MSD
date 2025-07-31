namespace Core.DTOs.WorkPackages;

/// <summary>
/// DTO for updating a Work Package
/// </summary>
public class UpdateWorkPackageDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ResponsibleUserId { get; set; }
    public Guid? DisciplineId { get; set; }
    public decimal? Budget { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public decimal? WeightFactor { get; set; }
}
