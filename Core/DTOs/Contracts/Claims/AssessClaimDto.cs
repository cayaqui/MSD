namespace Core.DTOs.Contracts.Claims;

public class AssessClaimDto
{
    public decimal AssessedAmount { get; set; }
    public int AssessedTimeExtension { get; set; }
    public bool HasMerit { get; set; }
    public decimal LiabilityPercentage { get; set; }
    public string AssessmentComments { get; set; } = string.Empty;
}
