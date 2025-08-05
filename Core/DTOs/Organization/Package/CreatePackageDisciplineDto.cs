namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for creating a new package discipline
/// </summary>
public class CreatePackageDisciplineDto
{
    public Guid DisciplineId { get; set; }
    public decimal EstimatedHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public bool IsLeadDiscipline { get; set; }
    public Guid? LeadEngineerId { get; set; }
    public string? Notes { get; set; }
    public string Currency { get; set; } = "USD";
}