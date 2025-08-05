namespace Core.DTOs.Organization.Company;

/// <summary>
/// DTO for updating company business information
/// </summary>
public class UpdateCompanyBusinessInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TaxId { get; set; } = string.Empty;
}