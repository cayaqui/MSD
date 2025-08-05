namespace Core.DTOs.Risk.Risk;

public class RiskExposureDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime CalculatedDate { get; set; }
    
    // Inherent Risk (before mitigation)
    public decimal InherentCostExposure { get; set; }
    public int InherentScheduleExposure { get; set; }
    public decimal InherentQualityScore { get; set; }
    
    // Current Risk (with current mitigations)
    public decimal CurrentCostExposure { get; set; }
    public int CurrentScheduleExposure { get; set; }
    public decimal CurrentQualityScore { get; set; }
    
    // Residual Risk (after all planned mitigations)
    public decimal ResidualCostExposure { get; set; }
    public int ResidualScheduleExposure { get; set; }
    public decimal ResidualQualityScore { get; set; }
    
    // Mitigation Effectiveness
    public decimal CostMitigationEffectiveness { get; set; }
    public decimal ScheduleMitigationEffectiveness { get; set; }
    public decimal QualityMitigationEffectiveness { get; set; }
    public decimal OverallMitigationEffectiveness { get; set; }
    
    // Risk Appetite Comparison
    public decimal ProjectRiskAppetite { get; set; }
    public bool ExceedsRiskAppetite { get; set; }
    public decimal RiskAppetiteUtilization { get; set; }
    
    // Breakdown by Category
    public List<CategoryExposureDto> ExposureByCategory { get; set; } = new();
    
    // Top Contributors
    public List<RiskContributorDto> TopCostContributors { get; set; } = new();
    public List<RiskContributorDto> TopScheduleContributors { get; set; } = new();
}

public class CategoryExposureDto
{
    public string Category { get; set; } = string.Empty;
    public decimal CostExposure { get; set; }
    public int ScheduleExposure { get; set; }
    public int RiskCount { get; set; }
    public decimal PercentageOfTotal { get; set; }
}

public class RiskContributorDto
{
    public Guid RiskId { get; set; }
    public string RiskCode { get; set; } = string.Empty;
    public string RiskTitle { get; set; } = string.Empty;
    public decimal Contribution { get; set; }
    public decimal PercentageOfTotal { get; set; }
}