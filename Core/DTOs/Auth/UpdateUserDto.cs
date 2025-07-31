namespace Core.DTOs.Auth;

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? CompanyId { get; set; }
}
