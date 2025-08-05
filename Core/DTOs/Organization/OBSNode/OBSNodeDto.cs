namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// Organizational Breakdown Structure node data transfer object
/// </summary>
public class OBSNodeDto
{
    public Guid Id { get; set; }
    
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string NodeType { get; set; } = "Department"; // Company, Division, Department, Team, Role
    
    // Hierarchical structure
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public int Level { get; set; }
    public string HierarchyPath { get; set; } = string.Empty;
    public bool HasChildren { get; set; }
    
    // Project association
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectCode { get; set; }
    
    // Manager/responsible person
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public string? ManagerEmail { get; set; }
    
    // Cost center for accounting
    public string? CostCenter { get; set; }
    
    // Resource capacity
    public decimal? TotalFTE { get; set; }
    public decimal? AvailableFTE { get; set; }
    public decimal UtilizationRate { get; set; }
    
    // Status
    public bool IsActive { get; set; }
    
    // Member count
    public int MemberCount { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navigation
    public List<OBSNodeDto> Children { get; set; } = new List<OBSNodeDto>();
}