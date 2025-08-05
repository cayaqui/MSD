namespace Core.DTOs.Risk.Risk;

public class RiskHeatMapDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    
    public List<HeatMapCell> Cells { get; set; } = new();
    public HeatMapLegend Legend { get; set; } = new();
    public HeatMapStatistics Statistics { get; set; } = new();
}

public class HeatMapCell
{
    public int Probability { get; set; }
    public int Impact { get; set; }
    public int RiskCount { get; set; }
    public decimal TotalExposure { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public List<HeatMapRiskItem> Risks { get; set; } = new();
}

public class HeatMapRiskItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal? CostImpact { get; set; }
    public int? ScheduleImpact { get; set; }
}

public class HeatMapLegend
{
    public List<LegendItem> ProbabilityLevels { get; set; } = new();
    public List<LegendItem> ImpactLevels { get; set; } = new();
    public List<LegendItem> RiskLevels { get; set; } = new();
}

public class LegendItem
{
    public int Value { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class HeatMapStatistics
{
    public int TotalRisks { get; set; }
    public decimal AverageRiskScore { get; set; }
    public string MostPopulatedCell { get; set; } = string.Empty;
    public decimal HighestExposureCell { get; set; }
    public Dictionary<string, int> RiskDistribution { get; set; } = new();
}