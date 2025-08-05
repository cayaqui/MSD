namespace Core.DTOs.Risk.Risk;

public class RiskDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    
    // Risk Assessment
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Threat, Opportunity
    public int Probability { get; set; } // 1-5
    public int Impact { get; set; } // 1-5
    public int RiskScore { get; set; } // Probability * Impact
    public string RiskLevel { get; set; } = string.Empty; // Low, Medium, High, Critical
    
    // Risk Details
    public string Cause { get; set; } = string.Empty;
    public string Effect { get; set; } = string.Empty;
    public decimal? CostImpact { get; set; }
    public int? ScheduleImpact { get; set; } // Days
    public string QualityImpact { get; set; } = string.Empty;
    
    // Risk Response
    public string ResponseStrategy { get; set; } = string.Empty; // Avoid, Transfer, Mitigate, Accept
    public string ResponsePlan { get; set; } = string.Empty;
    public Guid? ResponseOwnerId { get; set; }
    public string ResponseOwnerName { get; set; } = string.Empty;
    public DateTime? ResponseDueDate { get; set; }
    public decimal? ResponseCost { get; set; }
    
    // Residual Risk (after response)
    public int? ResidualProbability { get; set; }
    public int? ResidualImpact { get; set; }
    public int? ResidualRiskScore { get; set; }
    public string? ResidualRiskLevel { get; set; }
    
    // Status and Tracking
    public string Status { get; set; } = string.Empty; // Identified, Assessed, Planned, InProgress, Closed
    public DateTime IdentifiedDate { get; set; }
    public Guid IdentifiedById { get; set; }
    public string IdentifiedByName { get; set; } = string.Empty;
    public DateTime? ClosedDate { get; set; }
    public string? ClosureReason { get; set; }
    
    // Monitoring
    public DateTime? LastReviewDate { get; set; }
    public DateTime? NextReviewDate { get; set; }
    public string TriggerIndicators { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
    // Audit
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}