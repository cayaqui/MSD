namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for updating package discipline information
/// </summary>
public class UpdatePackageDisciplineDto
{
    public decimal EstimatedHours { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal ActualHours { get; set; }
    public decimal ActualCost { get; set; }
    public bool IsLeadDiscipline { get; set; }
    public Guid? LeadEngineerId { get; set; }
    public string? Notes { get; set; }
    public decimal ProgressPercentage { get; set; }
}