namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// DTO for OBS hierarchy tree
/// </summary>
public class OBSNodeTreeDto
{
    /// <summary>
    /// Project ID if filtered by project
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// Root nodes in the hierarchy
    /// </summary>
    public List<OBSNodeHierarchyDto> RootNodes { get; set; } = new();

    /// <summary>
    /// Total node count
    /// </summary>
    public int TotalNodeCount => GetTotalNodeCount();

    private int GetTotalNodeCount()
    {
        var count = RootNodes.Count;
        foreach (var root in RootNodes)
        {
            count += root.DescendantCount;
        }
        return count;
    }
}