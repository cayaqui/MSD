using Core.DTOs.Auth.ProjectTeamMembers;

namespace Core.DTOs.Organization.OBSNode;

/// <summary>
/// OBS node team data transfer object
/// </summary>
public class OBSNodeTeamDto : OBSNodeDto
{
    public List<ProjectTeamMemberDto> TeamMembers { get; set; } = new();
    public int TotalMembers { get; set; }
    public decimal TotalAllocation { get; set; }
}