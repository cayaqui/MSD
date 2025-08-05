namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// DTO for OBS node with hierarchical structure
/// </summary>
public class OBSNodeHierarchyDto : OBSNodeDto
{
    /// <summary>
    /// Child nodes
    /// </summary>
    public List<OBSNodeHierarchyDto> Children { get; set; } = new();

    /// <summary>
    /// Number of descendants
    /// </summary>
    public int DescendantCount => GetDescendantCount();

    private int GetDescendantCount()
    {
        var count = Children.Count;
        foreach (var child in Children)
        {
            count += child.GetDescendantCount();
        }
        return count;
    }
}