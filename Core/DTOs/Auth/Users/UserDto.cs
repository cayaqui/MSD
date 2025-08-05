// Auth/UserDto.cs

// Auth/UserDto.cs

// Auth/UserDto.cs

// Auth/UserDto.cs
using Core.DTOs.Auth.ProjectTeamMembers;

namespace Core.DTOs.Auth.Users;
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
    public string? Department { get; set; }
    public string? OfficeLocation { get; set; }
    public string? MobilePhone { get; set; }
    public string? BusinessPhone { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PreferredLanguage { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int LoginCount { get; set; }
    public List<ProjectTeamMemberDto> ProjectTeamMembers { get; set; } = new();
    public string DisplayName => string.IsNullOrEmpty(Name) ? Email : Name;
    public string FirstName => GivenName ?? string.Empty;
    public string LastName => Surname ?? string.Empty;
}
