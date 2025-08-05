namespace Core.DTOs.Reports;

public class RiskRegisterReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    
    // Risk Statistics
    public int TotalRisks { get; set; }
    public int ActiveRisks { get; set; }
    public int ClosedRisks { get; set; }
    public int RealizedRisks { get; set; }
    public int NewRisksThisPeriod { get; set; }
    public int ClosedRisksThisPeriod { get; set; }
    
    // Risk by Status
    public int IdentifiedRisks { get; set; }
    public int AnalyzedRisks { get; set; }
    public int MitigatingRisks { get; set; }
    public int MonitoringRisks { get; set; }
    
    // Risk by Category
    public List<RiskCategoryStatDto> RisksByCategory { get; set; } = new();
    
    // Risk by Impact/Probability
    public RiskMatrixSummaryDto RiskMatrix { get; set; } = new();
    
    // Financial Impact
    public decimal TotalPotentialImpact { get; set; }
    public decimal TotalMitigationCost { get; set; }
    public decimal TotalRealizedImpact { get; set; }
    public decimal ContingencyReserve { get; set; }
    public decimal ContingencyUsed { get; set; }
    
    // Top Risks
    public List<RiskDetailDto> TopRisks { get; set; } = new();
    
    // All Risks (grouped by category)
    public List<RiskCategoryGroupDto> RisksByGroup { get; set; } = new();
    
    // Risk Trends
    public List<RiskTrendDataDto> TrendData { get; set; } = new();
    
    // Risk Response Effectiveness
    public decimal ResponseEffectivenessRate { get; set; }
    public int SuccessfulMitigations { get; set; }
    public int FailedMitigations { get; set; }
    
    // Executive Summary
    public string ExecutiveSummary { get; set; } = string.Empty;
    public List<string> KeyConcerns { get; set; } = new();
    public List<string> RecommendedActions { get; set; } = new();
    
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class RiskCategoryStatDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalImpact { get; set; }
    public decimal AverageScore { get; set; }
    public int HighRisks { get; set; }
    public int MediumRisks { get; set; }
    public int LowRisks { get; set; }
}

public class RiskMatrixSummaryDto
{
    public int VeryHighCount { get; set; }
    public int HighCount { get; set; }
    public int MediumCount { get; set; }
    public int LowCount { get; set; }
    public int VeryLowCount { get; set; }
    public List<RiskMatrixCellDto> MatrixCells { get; set; } = new();
}

public class RiskMatrixCellDto
{
    public string Impact { get; set; } = string.Empty; // VeryLow, Low, Medium, High, VeryHigh
    public string Probability { get; set; } = string.Empty; // VeryLow, Low, Medium, High, VeryHigh
    public int Count { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public List<string> RiskIds { get; set; } = new();
}

public class RiskDetailDto
{
    public string RiskId { get; set; } = string.Empty;
    public string RiskTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime IdentifiedDate { get; set; }
    public string IdentifiedBy { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    
    // Risk Assessment
    public string Probability { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
    public decimal RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty; // VeryHigh, High, Medium, Low, VeryLow
    public decimal? FinancialImpact { get; set; }
    public int? ScheduleImpact { get; set; } // in days
    
    // Risk Response
    public string ResponseStrategy { get; set; } = string.Empty; // Avoid, Transfer, Mitigate, Accept
    public string ResponsePlan { get; set; } = string.Empty;
    public decimal? MitigationCost { get; set; }
    public string ResponseStatus { get; set; } = string.Empty;
    public DateTime? ResponseDueDate { get; set; }
    
    // Triggers and Indicators
    public List<string> Triggers { get; set; } = new();
    public List<string> EarlyWarningIndicators { get; set; } = new();
    
    // Updates
    public DateTime LastReviewDate { get; set; }
    public string LastUpdateBy { get; set; } = string.Empty;
    public List<RiskUpdateDto> RecentUpdates { get; set; } = new();
}

public class RiskCategoryGroupDto
{
    public string CategoryName { get; set; } = string.Empty;
    public List<RiskDetailDto> Risks { get; set; } = new();
}

public class RiskTrendDataDto
{
    public DateTime Period { get; set; }
    public int TotalRisks { get; set; }
    public int HighRisks { get; set; }
    public int MediumRisks { get; set; }
    public int LowRisks { get; set; }
    public int NewRisks { get; set; }
    public int ClosedRisks { get; set; }
    public int RealizedRisks { get; set; }
    public decimal AverageRiskScore { get; set; }
}

public class RiskUpdateDto
{
    public DateTime UpdateDate { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public string UpdateType { get; set; } = string.Empty; // Status, Assessment, Response
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
}