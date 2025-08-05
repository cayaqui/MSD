namespace Core.DTOs.Risk.Risk;

public class RiskRegisterDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    
    public List<RiskRegisterItemDto> Risks { get; set; } = new();
    public RiskRegisterSummaryDto Summary { get; set; } = new();
}

public class RiskRegisterItemDto
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    public int Probability { get; set; }
    public int Impact { get; set; }
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    
    public string ResponseStrategy { get; set; } = string.Empty;
    public string ResponseOwner { get; set; } = string.Empty;
    public DateTime? ResponseDueDate { get; set; }
    
    public decimal? CostExposure { get; set; }
    public int? ScheduleExposure { get; set; }
    
    public DateTime IdentifiedDate { get; set; }
    public DateTime? LastReviewDate { get; set; }
    public DateTime? NextReviewDate { get; set; }
}

public class RiskRegisterSummaryDto
{
    public int TotalRisks { get; set; }
    public decimal TotalCostExposure { get; set; }
    public int TotalScheduleExposure { get; set; }
    
    public Dictionary<string, int> RisksByCategory { get; set; } = new();
    public Dictionary<string, int> RisksByStatus { get; set; } = new();
    public Dictionary<string, int> RisksByLevel { get; set; } = new();
    public Dictionary<string, int> ResponseStrategies { get; set; } = new();
}