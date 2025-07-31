// Auth/UserDto.cs
namespace Core.DTOs.Auth;

/// <summary>
/// DTO for assigning a user to a project
/// </summary>
public class AssignUserToProjectDto
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public string Role { get; set; } = string.Empty;
    public decimal? AllocationPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

