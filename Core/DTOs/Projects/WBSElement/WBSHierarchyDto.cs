namespace Core.DTOs.Projects.WBSElement;

public class WBSHierarchyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Level { get; set; }
    public Guid? ParentId { get; set; }
    public List<WBSHierarchyDto> Children { get; set; } = new();
    
    // Additional properties for hierarchy view
    public bool IsExpanded { get; set; } = true;
    public bool IsLeaf => !Children.Any();
    public string Path { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    
    // Aggregate information
    public decimal TotalBudget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal Progress { get; set; }
    public int ActivityCount { get; set; }
    public int CompletedActivityCount { get; set; }
}