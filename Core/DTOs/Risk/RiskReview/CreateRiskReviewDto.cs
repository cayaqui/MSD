using Core.Enums.Risk;

namespace Core.DTOs.Risk.RiskReview;

public class CreateRiskReviewDto
{
    public Guid RiskId { get; set; }
    public int NewProbability { get; set; }
    public int NewImpact { get; set; }
    public string ReviewNotes { get; set; } = string.Empty;
    public string? ChangesIdentified { get; set; }
    public string? ActionsRequired { get; set; }
    public RiskStatus? NewStatus { get; set; }
    public bool ResponseStrategyChanged { get; set; }
    public string? UpdatedResponsePlan { get; set; }
    public DateTime? NextReviewDate { get; set; }
}