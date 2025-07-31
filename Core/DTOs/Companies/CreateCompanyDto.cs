namespace Core.DTOs.Companies;

/// <summary>
/// DTO for creating a new company
/// </summary>
public class CreateCompanyDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DefaultCurrency { get; set; } = "USD";
}
