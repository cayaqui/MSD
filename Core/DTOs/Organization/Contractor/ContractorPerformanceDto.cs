namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// Contractor performance data transfer object
/// </summary>
public class ContractorPerformanceDto
{
    public Guid ContractorId { get; set; }
    public string ContractorName { get; set; } = string.Empty;
    
    // Performance Metrics
    public decimal OverallRating { get; set; }
    public decimal QualityRating { get; set; }
    public decimal SafetyRating { get; set; }
    public decimal ScheduleRating { get; set; }
    public decimal CostRating { get; set; }
    public decimal CommunicationRating { get; set; }
    
    // Statistics
    public int TotalProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int DelayedProjects { get; set; }
    
    // Financial Performance
    public decimal TotalContractValue { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalPendingAmount { get; set; }
    public decimal AverageCostVariance { get; set; }
    
    // Schedule Performance
    public decimal AverageScheduleVariance { get; set; }
    public int OnTimeCompletions { get; set; }
    public int LateCompletions { get; set; }
    
    // Quality & Safety
    public int QualityIssues { get; set; }
    public int SafetyIncidents { get; set; }
    public int NonConformances { get; set; }
    
    // Evaluation History
    public DateTime? LastEvaluationDate { get; set; }
    public int TotalEvaluations { get; set; }
}