namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for updating discipline estimate
/// </summary>
public class UpdateDisciplineEstimateDto
{
    public decimal EstimatedHours { get; set; }
    public decimal EstimatedCost { get; set; }
}