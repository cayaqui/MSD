namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// DTO for adding a member to an OBS node
/// </summary>
public class AddOBSNodeMemberDto
{
    public Guid UserId { get; set; }
    public decimal? AllocationPercentage { get; set; }
}