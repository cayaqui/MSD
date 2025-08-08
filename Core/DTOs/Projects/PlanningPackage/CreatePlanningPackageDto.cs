namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for creating a new planning package
/// </summary>
public class CreatePlanningPackageDto
{
    public Guid ProjectId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ControlAccountId { get; set; }
    public decimal TotalBudget { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public List<CreatePlanningPackageEntryDto>? InitialEntries { get; set; }
}