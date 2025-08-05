// Auth/UserDto.cs
namespace Core.DTOs.Auth.ProjectTeamMembers;

/// <summary>
/// DTO for project team member information
/// </summary>
public class ProjectTeamMemberDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public decimal? AllocationPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

