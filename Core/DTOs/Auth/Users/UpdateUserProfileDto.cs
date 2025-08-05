namespace Core.DTOs.Auth.Users;

/// <summary>
/// DTO for updating user profile information
/// </summary>
public class UpdateUserProfileDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public string? Location { get; set; }
    public string? TimeZone { get; set; }
    public string? PreferredLanguage { get; set; }
    public byte[]? ProfilePicture { get; set; }
}