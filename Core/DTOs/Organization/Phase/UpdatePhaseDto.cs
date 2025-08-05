namespace Core.DTOs.Organization.Phase;

/// <summary>
/// Update phase data transfer object
/// </summary>
public class UpdatePhaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Sequence { get; set; }
    public int SequenceNumber { get; set; } // Compatibility with Projects/Projects
    
    // Schedule
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    // Budget
    public decimal PlannedBudget { get; set; }
    public decimal ApprovedBudget { get; set; }
    public decimal? ActualBudget { get; set; }
    public decimal? ForecastBudget { get; set; }
    
    // Weight and Gates
    public decimal WeightFactor { get; set; }
    public bool RequiresGateApproval { get; set; }
    
    // Progress
    public decimal ProgressPercentage { get; set; }
    public Core.Enums.Projects.PhaseStatus Status { get; set; }
    
    // Key Information
    public string? KeyDeliverables { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string? Assumptions { get; set; }
    public string? Risks { get; set; }
}