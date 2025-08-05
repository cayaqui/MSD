namespace Core.DTOs.Auth.Users;

public class CreateUserDto
{
    public string EntraId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? PreferredLanguage { get; set; }
}
