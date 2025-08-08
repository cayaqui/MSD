namespace Core.DTOs.Visualization;

public class RiskMatrixDto
{
    public string Title { get; set; } = "Risk Matrix";
    public List<RiskMatrixCellDto> Cells { get; set; } = new();
    public List<RiskItemDto> Risks { get; set; } = new();
    public RiskMatrixConfigDto Config { get; set; } = new();
}

public class RiskMatrixCellDto
{
    public int ProbabilityLevel { get; set; }
    public int ImpactLevel { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int RiskCount { get; set; }
    public List<Guid> RiskIds { get; set; } = new();
}

public class RiskItemDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int ProbabilityLevel { get; set; }
    public int ImpactLevel { get; set; }
    public decimal RiskScore { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public bool HasMitigation { get; set; }
}

public class RiskMatrixConfigDto
{
    public int MatrixSize { get; set; } = 5; // 3x3, 4x4, 5x5
    public bool ShowRiskCodes { get; set; } = true;
    public bool ShowRiskCount { get; set; } = true;
    public string[] ProbabilityLabels { get; set; } = { "Very Low", "Low", "Medium", "High", "Very High" };
    public string[] ImpactLabels { get; set; } = { "Negligible", "Minor", "Moderate", "Major", "Severe" };
}