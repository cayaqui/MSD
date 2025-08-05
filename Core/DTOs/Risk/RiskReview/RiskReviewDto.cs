namespace Core.DTOs.Risk.RiskReview;

public class RiskReviewDto
{
    public Guid Id { get; set; }
    public Guid RiskId { get; set; }
    public string RiskCode { get; set; } = string.Empty;
    public string RiskTitle { get; set; } = string.Empty;
    
    public DateTime ReviewDate { get; set; }
    public Guid ReviewedById { get; set; }
    public string ReviewedByName { get; set; } = string.Empty;
    
    public int PreviousProbability { get; set; }
    public int PreviousImpact { get; set; }
    public int NewProbability { get; set; }
    public int NewImpact { get; set; }
    
    public string ReviewNotes { get; set; } = string.Empty;
    public string ChangesIdentified { get; set; } = string.Empty;
    public string ActionsRequired { get; set; } = string.Empty;
    
    public bool StatusChanged { get; set; }
    public string? PreviousStatus { get; set; }
    public string? NewStatus { get; set; }
    
    public bool ResponseStrategyChanged { get; set; }
    public string? UpdatedResponsePlan { get; set; }
    
    public DateTime? NextReviewDate { get; set; }
}