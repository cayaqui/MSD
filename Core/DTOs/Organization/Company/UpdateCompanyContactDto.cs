namespace Core.DTOs.Organization.Company;

/// <summary>
/// DTO for updating company contact information
/// </summary>
public class UpdateCompanyContactDto
{
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
}