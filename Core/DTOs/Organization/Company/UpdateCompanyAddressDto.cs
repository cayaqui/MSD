namespace Core.DTOs.Organization.Company;

/// <summary>
/// DTO for updating company address information
/// </summary>
public class UpdateCompanyAddressDto
{
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
}