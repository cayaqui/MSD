namespace Core.DTOs.Reports;

public class RiskMatrixReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    
    // Matrix Configuration
    public List<string> ImpactLevels { get; set; } = new() { "VeryLow", "Low", "Medium", "High", "VeryHigh" };
    public List<string> ProbabilityLevels { get; set; } = new() { "VeryLow", "Low", "Medium", "High", "VeryHigh" };
    
    // Risk Distribution Summary
    public int TotalActiveRisks { get; set; }
    public RiskDistributionDto Distribution { get; set; } = new();
    
    // Heat Map Data
    public List<RiskMatrixRowDto> MatrixRows { get; set; } = new();
    
    // Risk Movement
    public List<RiskMovementDto> RiskMovements { get; set; } = new();
    
    // Risk Scores
    public decimal AverageRiskScore { get; set; }
    public decimal HighestRiskScore { get; set; }
    public decimal TotalRiskExposure { get; set; }
    
    // Top Risks by Quadrant
    public List<RiskQuadrantDto> Quadrants { get; set; } = new();
    
    // Risk Tolerance Thresholds
    public RiskToleranceDto Tolerance { get; set; } = new();
    
    // Trend Analysis
    public RiskMatrixTrendDto Trend { get; set; } = new();
    
    // Action Items
    public List<RiskActionItemDto> RequiredActions { get; set; } = new();
    
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class RiskDistributionDto
{
    public int CriticalRisks { get; set; } // Very High Impact & Probability
    public int HighRisks { get; set; }
    public int MediumRisks { get; set; }
    public int LowRisks { get; set; }
    public int NegligibleRisks { get; set; } // Very Low Impact & Probability
    
    public decimal CriticalPercentage { get; set; }
    public decimal HighPercentage { get; set; }
    public decimal MediumPercentage { get; set; }
    public decimal LowPercentage { get; set; }
    public decimal NegligiblePercentage { get; set; }
}

public class RiskMatrixRowDto
{
    public string ImpactLevel { get; set; } = string.Empty;
    public List<RiskMatrixCellDetailDto> Cells { get; set; } = new();
}

public class RiskMatrixCellDetailDto
{
    public string ProbabilityLevel { get; set; } = string.Empty;
    public int RiskCount { get; set; }
    public string RiskLevel { get; set; } = string.Empty; // Critical, High, Medium, Low, Negligible
    public string CellColor { get; set; } = string.Empty; // Red, Orange, Yellow, Green
    public decimal TotalExposure { get; set; }
    public List<RiskSummaryDto> Risks { get; set; } = new();
}

public class RiskMovementDto
{
    public string RiskId { get; set; } = string.Empty;
    public string RiskTitle { get; set; } = string.Empty;
    public string PreviousImpact { get; set; } = string.Empty;
    public string CurrentImpact { get; set; } = string.Empty;
    public string PreviousProbability { get; set; } = string.Empty;
    public string CurrentProbability { get; set; } = string.Empty;
    public string MovementType { get; set; } = string.Empty; // Improved, Deteriorated, New, Closed
    public string Reason { get; set; } = string.Empty;
    public DateTime MovementDate { get; set; }
}

public class RiskQuadrantDto
{
    public string QuadrantName { get; set; } = string.Empty; // Critical, Major, Moderate, Minor
    public string Description { get; set; } = string.Empty;
    public int RiskCount { get; set; }
    public List<TopRiskDto> TopRisks { get; set; } = new();
    public string RecommendedStrategy { get; set; } = string.Empty;
}

public class TopRiskDto
{
    public string RiskId { get; set; } = string.Empty;
    public string RiskTitle { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
    public string Probability { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string ResponseStrategy { get; set; } = string.Empty;
    public string ResponseStatus { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public decimal Exposure { get; set; }
    public string Owner { get; set; } = string.Empty;
}

public class RiskToleranceDto
{
    public decimal AcceptableThreshold { get; set; }
    public decimal TolerableThreshold { get; set; }
    public decimal UnacceptableThreshold { get; set; }
    public int RisksAboveTolerance { get; set; }
    public int RisksAtTolerance { get; set; }
    public int RisksBelowTolerance { get; set; }
    public List<string> RisksRequiringEscalation { get; set; } = new();
}

public class RiskMatrixTrendDto
{
    public string TrendDirection { get; set; } = string.Empty; // Improving, Stable, Worsening
    public decimal ScoreChangeFromLastPeriod { get; set; }
    public int NewRisksAdded { get; set; }
    public int RisksClosed { get; set; }
    public int RisksEscalated { get; set; }
    public int RisksDeescalated { get; set; }
    public List<MonthlyTrendDto> MonthlyData { get; set; } = new();
}

public class MonthlyTrendDto
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal AverageScore { get; set; }
    public int CriticalCount { get; set; }
    public int HighCount { get; set; }
    public int MediumCount { get; set; }
    public int LowCount { get; set; }
}

public class RiskActionItemDto
{
    public string RiskId { get; set; } = string.Empty;
    public string RiskTitle { get; set; } = string.Empty;
    public string ActionRequired { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty; // Immediate, High, Medium, Low
    public string ResponsibleParty { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
}