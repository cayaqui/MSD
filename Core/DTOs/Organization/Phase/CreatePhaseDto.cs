using Core.Enums.Projects;

namespace Core.DTOs.Organization.Phase;

/// <summary>
/// Create phase data transfer object
/// </summary>
public class CreatePhaseDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Foreign Key
    public Guid ProjectId { get; set; }
    
    // Phase Information
    public int Sequence { get; set; }
    public int SequenceNumber { get; set; } // Compatibility with Projects/Projects
    public PhaseType PhaseType { get; set; }
    
    // Schedule
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    
    // Budget
    public decimal PlannedBudget { get; set; }
    public decimal ApprovedBudget { get; set; }
    
    // Weight and Gates
    public decimal WeightFactor { get; set; } = 1.0m;
    public bool RequiresGateReview { get; set; }
    public bool RequiresGateApproval { get; set; } // Compatibility with Projects/Projects
    
    // Key Information
    public string? KeyDeliverables { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string? Assumptions { get; set; }
    public string? Risks { get; set; }
}