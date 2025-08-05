namespace Core.DTOs.Risk.Risk;

public class UpdateRiskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Risk Assessment
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Probability { get; set; }
    public int Impact { get; set; }
    
    // Risk Details
    public string Cause { get; set; } = string.Empty;
    public string Effect { get; set; } = string.Empty;
    public decimal? CostImpact { get; set; }
    public int? ScheduleImpact { get; set; }
    public string? QualityImpact { get; set; }
    
    // Risk Response
    public string? ResponseStrategy { get; set; }
    public string? ResponsePlan { get; set; }
    public Guid? ResponseOwnerId { get; set; }
    public DateTime? ResponseDueDate { get; set; }
    public decimal? ResponseCost { get; set; }
    
    // Monitoring
    public string? TriggerIndicators { get; set; }
    public DateTime? NextReviewDate { get; set; }
}