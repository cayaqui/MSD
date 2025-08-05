namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// DTO for adding a member to an OBS node
/// </summary>
public class AddOBSMemberDto
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public decimal AllocationPercentage { get; set; } = 100;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}