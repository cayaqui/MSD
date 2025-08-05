namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// Detailed OBS node information including hierarchy
/// </summary>
public class OBSNodeDetailDto : OBSNodeDto
{
    public List<OBSNodeDto> Ancestors { get; set; } = new();
    public List<OBSNodeDetailDto> Children { get; set; } = new();
    public int DescendantCount { get; set; }
    public decimal TotalCapacity { get; set; }
    public decimal TotalUtilization { get; set; }
}