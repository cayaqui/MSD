namespace Core.DTOs.Cost;

/// <summary>
/// DTO for creating Work Package from Planning Package
/// </summary>
public class CreateWorkPackageFromPlanningDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Budget { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public string? ResponsibleUserId { get; set; }
    public Guid? DisciplineId { get; set; }
}
