namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for updating package contract information
/// </summary>
public class UpdatePackageContractDto
{
    public string? ContractNumber { get; set; }
    public string? ContractType { get; set; }
    public decimal ContractValue { get; set; }
    public string Currency { get; set; } = "USD";
}