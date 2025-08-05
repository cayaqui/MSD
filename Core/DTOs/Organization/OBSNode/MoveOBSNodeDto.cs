namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// DTO for moving an OBS node in the hierarchy
/// </summary>
public class MoveOBSNodeDto
{
    public Guid NewParentId { get; set; }
}