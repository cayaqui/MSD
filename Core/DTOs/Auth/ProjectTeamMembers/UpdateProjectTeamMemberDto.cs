namespace Core.DTOs.Auth.ProjectTeamMembers;

/// <summary>
/// DTO for updating a project team member
/// </summary>
public class UpdateProjectTeamMemberDto
{
    /// <summary>
    /// The role of the team member in the project
    /// </summary>
    public string? Role { get; set; }
    
    /// <summary>
    /// Percentage of time allocated to this project (0-100)
    /// </summary>
    public decimal? AllocationPercentage { get; set; }
    
    /// <summary>
    /// When the assignment starts
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// When the assignment ends (optional)
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Specific responsibilities for this team member (optional)
    /// </summary>
    public string? Responsibilities { get; set; }
    
    /// <summary>
    /// Whether the team member is active on the project
    /// </summary>
    public bool? IsActive { get; set; }
}
