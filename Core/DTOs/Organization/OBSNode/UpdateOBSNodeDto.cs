namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// DTO for updating OBS node information
/// </summary>
public class UpdateOBSNodeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string NodeType { get; set; } = "Department";
    
    public Guid? ManagerId { get; set; }
    public string? CostCenter { get; set; }
    
    public decimal? TotalFTE { get; set; }
    public decimal? AvailableFTE { get; set; }
    
    public bool IsActive { get; set; }
}