namespace Core.DTOs.Organization.Company;

/// <summary>
/// DTO for updating an existing company
/// </summary>
public class UpdateCompanyDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? DefaultCurrency { get; set; }
}
