namespace Core.DTOs.Organization.RAM;

/// <summary>
/// DTO for updating RAM assignment
/// </summary>
public class UpdateRAMDto
{
    public string ResponsibilityType { get; set; } = string.Empty; // R, A, C, I, S, V
    
    public decimal AllocationPercentage { get; set; }
    public decimal PlannedManHours { get; set; }
    public decimal PlannedCost { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? ControlAccountId { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}