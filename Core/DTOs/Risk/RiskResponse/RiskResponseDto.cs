namespace Core.DTOs.Risk.RiskResponse;

public class RiskResponseDto
{
    public Guid Id { get; set; }
    public Guid RiskId { get; set; }
    public string RiskCode { get; set; } = string.Empty;
    public string RiskTitle { get; set; } = string.Empty;
    
    public string Strategy { get; set; } = string.Empty; // Avoid, Transfer, Mitigate, Accept, Escalate, Exploit, Share, Enhance
    public string Description { get; set; } = string.Empty;
    public string Actions { get; set; } = string.Empty;
    
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    
    // Expected outcome
    public int ExpectedProbability { get; set; }
    public int ExpectedImpact { get; set; }
    public int ExpectedRiskScore { get; set; }
    
    public string Status { get; set; } = string.Empty; // Planned, InProgress, Completed, Cancelled
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletionNotes { get; set; }
    
    public decimal EffectivenessScore { get; set; } // 0-100%
    public string? LessonsLearned { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}