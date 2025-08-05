namespace Core.DTOs.Organization.Project;

/// <summary>
/// DTO for assigning a user to a project as a team member
/// </summary>
public class AssignProjectTeamMemberDto
{
    /// <summary>
    /// The project to assign the user to
    /// </summary>
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// The user to assign to the project
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// The role the user will have in the project
    /// </summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// Percentage of time allocated to this project (0-100)
    /// </summary>
    public decimal AllocationPercentage { get; set; } = 100;
    
    /// <summary>
    /// When the assignment starts
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.Today;
    
    /// <summary>
    /// When the assignment ends (optional)
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Specific responsibilities for this team member (optional)
    /// </summary>
    public string? Responsibilities { get; set; }
}