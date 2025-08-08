namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for updating planning package entry
/// </summary>
public class UpdatePlanningPackageEntryDto
{
    public Guid? Id { get; set; } // If null, creates new entry
    public Guid PhaseId { get; set; }
    public decimal Budget { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; } // For marking entries for deletion
}