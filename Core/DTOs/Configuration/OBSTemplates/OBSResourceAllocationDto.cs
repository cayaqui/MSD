namespace Core.DTOs.Configuration.OBSTemplates;

public class OBSResourceAllocationDto
{
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty; // User, Equipment, etc.
    public decimal AllocationPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
}
