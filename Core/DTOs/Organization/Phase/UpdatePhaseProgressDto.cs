namespace Core.DTOs.Organization.Phase;

/// <summary>
/// DTO for updating phase progress
/// </summary>
public class UpdatePhaseProgressDto
{
    public decimal ProgressPercentage { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
}