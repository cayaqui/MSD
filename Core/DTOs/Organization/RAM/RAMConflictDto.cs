namespace Core.DTOs.Organization.RAM;

/// <summary>
/// DTO for RAM conflict
/// </summary>
public class RAMConflictDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public Guid? OBSNodeId { get; set; }
    public Guid? WBSElementId { get; set; }
}