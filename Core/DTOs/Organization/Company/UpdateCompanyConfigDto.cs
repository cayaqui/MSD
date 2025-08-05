namespace Core.DTOs.Organization.Company;

/// <summary>
/// DTO for updating company configuration
/// </summary>
public class UpdateCompanyConfigDto
{
    public string DefaultCurrency { get; set; } = "USD";
    public string? FiscalYearStart { get; set; } // MM-DD format
}