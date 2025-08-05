namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for displaying package discipline information
/// </summary>
public class PackageDisciplineDto
{
    public Guid Id { get; set; }
    
    // Foreign Keys
    public Guid PackageId { get; set; }
    public string PackageCode { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public Guid DisciplineId { get; set; }
    public string DisciplineCode { get; set; } = string.Empty;
    public string DisciplineName { get; set; } = string.Empty;
    
    // Work allocation
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public bool IsLeadDiscipline { get; set; }
    
    // Resource assignment
    public Guid? LeadEngineerId { get; set; }
    public string? LeadEngineerName { get; set; }
    public string? Notes { get; set; }
    
    // Progress
    public decimal ProgressPercentage { get; set; }
    public DateTime? LastProgressUpdate { get; set; }
    
    // Cost allocation
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public string Currency { get; set; } = "USD";
    
    // Calculated fields
    public decimal ProductivityRate { get; set; }
    public decimal CostVariance { get; set; }
    public decimal HoursVariance { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}