namespace Core.DTOs.Cost.WorkPackages;

public class UpdateWorkPackageDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public decimal Budget { get; set; }
}