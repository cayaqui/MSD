namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for creating planning package entries
/// </summary>
public class CreatePlanningPackageEntryDto
{
    public Guid PhaseId { get; set; }
    public decimal Budget { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Notes { get; set; }
}