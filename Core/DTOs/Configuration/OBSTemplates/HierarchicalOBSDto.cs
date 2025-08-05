using Core.DTOs.Organization.OBSNode;

namespace Core.DTOs.Configuration.OBSTemplates;

public class HierarchicalOBSDto
{
    public OBSNodeDto Node { get; set; } = new();
    public List<HierarchicalOBSDto> Children { get; set; } = new();
    public int TotalDescendants { get; set; }
}
