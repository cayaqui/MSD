using Core.Enums.Projects;

namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for updating package information
/// </summary>
public class UpdatePackageDto
{
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
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    // Progress
    public decimal ProgressPercentage { get; set; }
    
    // Foreign Keys
    public Guid? ContractorId { get; set; }
}