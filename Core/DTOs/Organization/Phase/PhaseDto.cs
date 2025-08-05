using Core.Enums.Projects;

namespace Core.DTOs.Organization.Phase;

/// <summary>
/// Phase data transfer object
/// </summary>
public class PhaseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Foreign Keys
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    
    // Phase Information
    public int Sequence { get; set; }
    public int SequenceNumber { get; set; } // Compatibility with Projects/Projects
    public PhaseType PhaseType { get; set; }
    public PhaseStatus Status { get; set; }
    
    // Schedule
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public DateTime? BaselineStartDate { get; set; }
    public DateTime? BaselineEndDate { get; set; }
    
    // Budget
    public decimal PlannedBudget { get; set; }
    public decimal ApprovedBudget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    
    // Progress
    public decimal ProgressPercentage { get; set; }
    public decimal WeightFactor { get; set; }
    
    // Gate Review
    public bool RequiresGateReview { get; set; }
    public bool RequiresGateApproval { get; set; } // Compatibility with Projects/Projects
    public DateTime? GateReviewDate { get; set; }
    public DateTime? GateApprovalDate { get; set; } // Compatibility with Projects/Projects
    public bool? GateReviewApproved { get; set; }
    public string? GateReviewNotes { get; set; }
    public string? GateApprovedBy { get; set; }
    
    // Key Information
    public string? KeyDeliverables { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string? Assumptions { get; set; }
    public string? Risks { get; set; }
    
    // Status
    public bool IsActive { get; set; }
    public bool IsOverBudget { get; set; }
    public bool IsDelayed { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Computed Properties
    public int PlannedDuration { get; set; }
    public int ActualDuration { get; set; }
    public int DaysRemaining { get; set; }
    public decimal CostVariance { get; set; }
    public decimal ScheduleVariance { get; set; }
}