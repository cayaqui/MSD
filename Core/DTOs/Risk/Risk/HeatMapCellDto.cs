namespace Core.DTOs.Risk.Risk;

public class HeatMapCellDto
{
    public string Category { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public int RiskCount { get; set; }
    public int TotalScore { get; set; }
    public List<RiskSummaryDto> Risks { get; set; } = new();
}