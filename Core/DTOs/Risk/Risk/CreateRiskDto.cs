namespace Core.DTOs.Risk.Risk;

public class CreateRiskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    
    // Risk Assessment
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Threat, Opportunity
    public int Probability { get; set; } // 1-5
    public int Impact { get; set; } // 1-5
    
    // Risk Details
    public string Cause { get; set; } = string.Empty;
    public string Effect { get; set; } = string.Empty;
    public decimal? CostImpact { get; set; }
    public int? ScheduleImpact { get; set; }
    public string? QualityImpact { get; set; }
    
    // Initial Response
    public string? ResponseStrategy { get; set; }
    public string? ResponsePlan { get; set; }
    public Guid? ResponseOwnerId { get; set; }
    public DateTime? ResponseDueDate { get; set; }
    
    // Monitoring
    public string? TriggerIndicators { get; set; }
    public DateTime? NextReviewDate { get; set; }
    
    // Identification
    public Guid IdentifiedById { get; set; }
    public string? IdentifiedByName { get; set; }
    public DateTime? IdentifiedDate { get; set; }
}