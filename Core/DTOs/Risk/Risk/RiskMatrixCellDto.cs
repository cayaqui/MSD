namespace Core.DTOs.Risk.Risk;

public class RiskMatrixCellDto
{
    public int Probability { get; set; }
    public int Impact { get; set; }
    public int RiskScore { get; set; }
    public int RiskCount { get; set; }
    public List<RiskSummaryDto> Risks { get; set; } = new();
}