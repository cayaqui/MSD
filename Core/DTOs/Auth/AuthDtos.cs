// Auth/UserDto.cs
namespace Core.DTOs.Auth;
/// <summary>
/// DTO for displaying user information
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string EntraId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PreferredLanguage { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int LoginCount { get; set; }
    public List<ProjectTeamMemberDto> ProjectTeamMembers { get; set; } = new();
    public string? DisplayName => string.IsNullOrEmpty(Name) ? Email : Name;
}
