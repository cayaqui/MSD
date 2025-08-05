namespace Core.DTOs.Risk.RiskResponse;

public class UpdateRiskResponseDto
{
    public string Strategy { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Actions { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public DateTime DueDate { get; set; }
    public decimal EstimatedCost { get; set; }
    public int ExpectedProbability { get; set; }
    public int ExpectedImpact { get; set; }
}