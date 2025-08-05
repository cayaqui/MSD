namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// DTO for OBS resource allocations
/// </summary>
public class OBSResourceAllocationDto
{
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public Guid NodeId { get; set; }
    public string NodeName { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public decimal AllocationPercentage { get; set; }
    public decimal AllocatedHours { get; set; }
    public decimal AvailableHours { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}