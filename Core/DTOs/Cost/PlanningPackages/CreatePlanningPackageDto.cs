namespace Core.DTOs.Cost.PlanningPackages;

/// <summary>
/// DTO for creating a Planning Package
/// </summary>
public class CreatePlanningPackageDto
{
    public Guid ControlAccountId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Budget { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public string Currency { get; set; } = "USD";
}
