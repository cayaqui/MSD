namespace Core.DTOs.Organization.Package;

/// <summary>
/// DTO for discipline performance summary
/// </summary>
public class DisciplinePerformanceSummaryDto
{
    public Guid Id { get; set; }
    public Guid PackageId { get; set; }
    public string PackageCode { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public Guid DisciplineId { get; set; }
    public string DisciplineCode { get; set; } = string.Empty;
    public string DisciplineName { get; set; } = string.Empty;
    
    // Performance Metrics
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal HoursVariance { get; set; }
    public decimal HoursVariancePercentage { get; set; }
    public decimal ProductivityRate { get; set; }
    
    // Cost Performance
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CostVariance { get; set; }
    public decimal CostVariancePercentage { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    
    // Progress
    public decimal ProgressPercentage { get; set; }
    public DateTime? LastProgressUpdate { get; set; }
    
    // Status
    public bool IsOnBudget { get; set; }
    public bool IsOnSchedule { get; set; }
    public bool IsLeadDiscipline { get; set; }
}