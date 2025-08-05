namespace Core.DTOs.Auth.Users;

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? CompanyId { get; set; }
    public string? Department { get; set; }
    public string? OfficeLocation { get; set; }
    public string? MobilePhone { get; set; }
    public string? BusinessPhone { get; set; }
    public string? PhotoUrl { get; set; }

    public bool IsActive { get; set; } = true;
}
