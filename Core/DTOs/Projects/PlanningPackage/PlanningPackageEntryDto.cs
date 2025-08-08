namespace Core.DTOs.Projects.PlanningPackage;

/// <summary>
/// DTO for planning package time-phased entry
/// </summary>
public class PlanningPackageEntryDto
{
    public Guid Id { get; set; }
    public Guid PlanningPackageId { get; set; }
    public Guid PhaseId { get; set; }
    public string PhaseName { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Notes { get; set; }
}