namespace Core.DTOs.Risk.Risk;

public class RiskMatrixDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    
    public List<RiskMatrixCell> Matrix { get; set; } = new();
    public RiskMatrixSummary Summary { get; set; } = new();
}

public class RiskMatrixCell
{
    public int Probability { get; set; }
    public int Impact { get; set; }
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public List<RiskMatrixItem> Risks { get; set; } = new();
}

public class RiskMatrixItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class RiskMatrixSummary
{
    public int TotalRisks { get; set; }
    public int ActiveRisks { get; set; }
    public int ClosedRisks { get; set; }
    
    public int CriticalRisks { get; set; }
    public int HighRisks { get; set; }
    public int MediumRisks { get; set; }
    public int LowRisks { get; set; }
    
    public int Threats { get; set; }
    public int Opportunities { get; set; }
    
    public decimal TotalCostExposure { get; set; }
    public int TotalScheduleExposure { get; set; }
}