using Core.Enums.Projects;

namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for displaying package information
/// </summary>
public class PackageDto
{
    public Guid Id { get; set; }
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
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    // Progress
    public decimal ProgressPercentage { get; set; }
    
    // Foreign Keys
    public Guid? PhaseId { get; set; }
    public string? PhaseName { get; set; }
    public Guid? WBSElementId { get; set; }
    public string? WBSElementName { get; set; }
    public Guid? ContractorId { get; set; }
    public string? ContractorName { get; set; }
    
    // Status
    public bool IsOverdue { get; set; }
    public int? DaysOverdue { get; set; }
    public decimal? ScheduleVariance { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}