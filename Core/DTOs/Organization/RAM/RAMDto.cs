namespace Core.DTOs.Organization.RAM;

/// <summary>
/// DTO for displaying Responsibility Assignment Matrix (RAM) information
/// </summary>
public class RAMDto
{
    public Guid Id { get; set; }
    
    // Project
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    
    // WBS Element
    public Guid WBSElementId { get; set; }
    public string WBSCode { get; set; } = string.Empty;
    public string WBSName { get; set; } = string.Empty;
    
    // OBS Node
    public Guid OBSNodeId { get; set; }
    public string OBSCode { get; set; } = string.Empty;
    public string OBSName { get; set; } = string.Empty;
    
    // RACI assignment
    public string ResponsibilityType { get; set; } = string.Empty; // R, A, C, I, S, V
    public string ResponsibilityDescription { get; set; } = string.Empty;
    
    // Additional details
    public decimal AllocationPercentage { get; set; }
    public decimal PlannedManHours { get; set; }
    public decimal PlannedCost { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Control Account linkage
    public Guid? ControlAccountId { get; set; }
    public string? ControlAccountCode { get; set; }
    
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}