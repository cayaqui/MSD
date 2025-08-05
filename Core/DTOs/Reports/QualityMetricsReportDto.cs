namespace Core.DTOs.Reports;

public class QualityMetricsReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    // Overall Quality Score
    public decimal OverallQualityScore { get; set; } // 0-100
    public string QualityRating { get; set; } = string.Empty; // Excellent, Good, Fair, Poor
    public decimal ScoreChangeFromLastPeriod { get; set; }
    
    // Non-Conformance Metrics
    public NonConformanceMetricsDto NonConformances { get; set; } = new();
    
    // Inspection and Testing
    public InspectionMetricsDto Inspections { get; set; } = new();
    
    // Quality Control Activities
    public QualityControlMetricsDto QualityControl { get; set; } = new();
    
    // Defect Metrics
    public DefectMetricsDto Defects { get; set; } = new();
    
    // Audit Results
    public AuditMetricsDto Audits { get; set; } = new();
    
    // Corrective Actions
    public CorrectiveActionMetricsDto CorrectiveActions { get; set; } = new();
    
    // Quality Cost Analysis
    public QualityCostDto QualityCosts { get; set; } = new();
    
    // Key Performance Indicators
    public List<QualityKPIDto> KeyIndicators { get; set; } = new();
    
    // Trend Analysis
    public List<QualityTrendDto> TrendData { get; set; } = new();
    
    // Executive Summary
    public string ExecutiveSummary { get; set; } = string.Empty;
    public List<string> Achievements { get; set; } = new();
    public List<string> AreasForImprovement { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class NonConformanceMetricsDto
{
    public int TotalNCRsRaised { get; set; }
    public int NCRsClosedThisPeriod { get; set; }
    public int OpenNCRs { get; set; }
    public int OverdueNCRs { get; set; }
    public decimal AverageClosureTime { get; set; } // in days
    public List<NCRByCategoryDto> NCRsByCategory { get; set; } = new();
    public List<NCRBySeverityDto> NCRsBySeverity { get; set; } = new();
}

public class NCRByCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public string Trend { get; set; } = string.Empty; // Increasing, Stable, Decreasing
}

public class NCRBySeverityDto
{
    public string Severity { get; set; } = string.Empty; // Critical, Major, Minor
    public int Count { get; set; }
    public decimal AverageResolutionTime { get; set; }
    public decimal CostImpact { get; set; }
}

public class InspectionMetricsDto
{
    public int TotalInspections { get; set; }
    public int PassedInspections { get; set; }
    public int FailedInspections { get; set; }
    public int ConditionalPasses { get; set; }
    public decimal PassRate { get; set; }
    public decimal FirstTimePassRate { get; set; }
    public List<InspectionByTypeDto> InspectionsByType { get; set; } = new();
}

public class InspectionByTypeDto
{
    public string InspectionType { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public decimal PassRate { get; set; }
    public List<string> CommonFailureReasons { get; set; } = new();
}

public class QualityControlMetricsDto
{
    public int QCPointsInspected { get; set; }
    public int QCPointsPassed { get; set; }
    public decimal ComplianceRate { get; set; }
    public int ChecklistsCompleted { get; set; }
    public decimal ChecklistCompletionRate { get; set; }
    public List<QCActivityDto> Activities { get; set; } = new();
}

public class QCActivityDto
{
    public string ActivityName { get; set; } = string.Empty;
    public int PlannedCount { get; set; }
    public int ActualCount { get; set; }
    public decimal CompletionRate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class DefectMetricsDto
{
    public int TotalDefectsFound { get; set; }
    public int DefectsFixed { get; set; }
    public int OpenDefects { get; set; }
    public decimal DefectDensity { get; set; } // defects per unit
    public decimal DefectRemovalEfficiency { get; set; }
    public List<DefectByPhaseDto> DefectsByPhase { get; set; } = new();
    public List<DefectByTypeDto> DefectsByType { get; set; } = new();
}

public class DefectByPhaseDto
{
    public string Phase { get; set; } = string.Empty;
    public int DefectsFound { get; set; }
    public int DefectsIntroduced { get; set; }
    public decimal CostToFix { get; set; }
}

public class DefectByTypeDto
{
    public string DefectType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public decimal AverageFixTime { get; set; }
}

public class AuditMetricsDto
{
    public int PlannedAudits { get; set; }
    public int CompletedAudits { get; set; }
    public int OverdueAudits { get; set; }
    public decimal AuditCompletionRate { get; set; }
    public int TotalFindings { get; set; }
    public int CriticalFindings { get; set; }
    public int MajorFindings { get; set; }
    public int MinorFindings { get; set; }
    public int Observations { get; set; }
    public List<AuditResultDto> AuditResults { get; set; } = new();
}

public class AuditResultDto
{
    public string AuditType { get; set; } = string.Empty;
    public DateTime AuditDate { get; set; }
    public string Auditor { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty; // Pass, Conditional Pass, Fail
    public int FindingsCount { get; set; }
    public decimal ComplianceScore { get; set; }
}

public class CorrectiveActionMetricsDto
{
    public int TotalCARsRaised { get; set; }
    public int CARsImplemented { get; set; }
    public int CARsOverdue { get; set; }
    public int CARsVerified { get; set; }
    public decimal EffectivenessRate { get; set; }
    public decimal AverageImplementationTime { get; set; }
    public List<CARStatusDto> CARsByStatus { get; set; } = new();
}

public class CARStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal AverageAge { get; set; } // in days
}

public class QualityCostDto
{
    public decimal PreventionCost { get; set; }
    public decimal AppraisalCost { get; set; }
    public decimal InternalFailureCost { get; set; }
    public decimal ExternalFailureCost { get; set; }
    public decimal TotalQualityCost { get; set; }
    public decimal CostOfGoodQuality { get; set; }
    public decimal CostOfPoorQuality { get; set; }
    public decimal QualityCostAsPercentageOfProjectCost { get; set; }
    public List<CostByActivityDto> CostBreakdown { get; set; } = new();
}

public class CostByActivityDto
{
    public string Activity { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string Category { get; set; } = string.Empty; // Prevention, Appraisal, Failure
    public decimal PercentageOfTotal { get; set; }
}

public class QualityKPIDto
{
    public string KPIName { get; set; } = string.Empty;
    public decimal Target { get; set; }
    public decimal Actual { get; set; }
    public decimal Variance { get; set; }
    public string Status { get; set; } = string.Empty; // OnTarget, BelowTarget, AboveTarget
    public string Trend { get; set; } = string.Empty; // Improving, Stable, Declining
    public string Unit { get; set; } = string.Empty;
}

public class QualityTrendDto
{
    public DateTime Period { get; set; }
    public decimal QualityScore { get; set; }
    public int NCRCount { get; set; }
    public decimal PassRate { get; set; }
    public decimal DefectDensity { get; set; }
    public decimal ComplianceRate { get; set; }
    public decimal QualityCost { get; set; }
}