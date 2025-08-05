namespace Core.DTOs.Organization.RAM;

/// <summary>
/// DTO for creating a new RAM assignment
/// </summary>
public class CreateRAMDto
{
    public Guid ProjectId { get; set; }
    public Guid WBSElementId { get; set; }
    public Guid OBSNodeId { get; set; }
    public string ResponsibilityType { get; set; } = "R"; // R, A, C, I, S, V
    
    public decimal AllocationPercentage { get; set; }
    public decimal PlannedManHours { get; set; }
    public decimal PlannedCost { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? ControlAccountId { get; set; }
    public string? Notes { get; set; }
}