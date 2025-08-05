namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// DTO for creating a new OBS node
/// </summary>
public class CreateOBSNodeDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string NodeType { get; set; } = "Department";
    
    public Guid? ParentId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? ManagerId { get; set; }
    
    public string? CostCenter { get; set; }
    
    public decimal? TotalFTE { get; set; }
    public decimal? AvailableFTE { get; set; }
}