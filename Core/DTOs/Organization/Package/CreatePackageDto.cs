using Core.Enums.Projects;

namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for creating a new package
/// </summary>
public class CreatePackageDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // WBS
    public string WBSCode { get; set; } = string.Empty;
    
    // Type
    public PackageType PackageType { get; set; }
    
    // Contract Information
    public string? ContractNumber { get; set; }
    public string? ContractType { get; set; }
    public decimal ContractValue { get; set; }
    public string Currency { get; set; } = "USD";
    
    // Dates
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    
    // Foreign Keys
    public Guid? PhaseId { get; set; }
    public Guid? WBSElementId { get; set; }
    public Guid? ContractorId { get; set; }
}