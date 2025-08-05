namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// DTO for updating contractor insurance information
/// </summary>
public class UpdateContractorInsuranceDto
{
    public DateTime ExpiryDate { get; set; }
    public decimal Amount { get; set; }
    public string Company { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
}