namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for time-phased budget entry
/// </summary>
public class TimePhasedBudgetDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Period { get; set; } = string.Empty; // e.g., "2024-01"
    public decimal Budget { get; set; }
    public Guid PhaseId { get; set; }
    public string PhaseName { get; set; } = string.Empty;
}