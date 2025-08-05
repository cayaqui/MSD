namespace Core.DTOs.Organization.Project;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string WBSCode { get; set; } = string.Empty;
    public Guid OperationId { get; set; }
    public string OperationName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal TotalBudget { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal? ActualCost { get; set; }
    public decimal ProgressPercentage { get; set; }
    public string? Location { get; set; }
    public string? Client { get; set; }
    public string? ContractNumber { get; set; }
    public bool IsActive { get; set; }
    public int TeamMemberCount { get; set; }
    public int PhaseCount { get; set; }
    
    // Audit properties
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}