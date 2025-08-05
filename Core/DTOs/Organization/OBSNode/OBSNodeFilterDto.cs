namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// Filter criteria for searching OBS nodes
/// </summary>
public class OBSNodeFilterDto
{
    public string? SearchTerm { get; set; }
    public Guid? ProjectId { get; set; }
    public string? NodeType { get; set; }
    public Guid? ManagerId { get; set; }
    public int? Level { get; set; }
    public bool? IsActive { get; set; }
    public bool IncludeInactive { get; set; } = false;
}