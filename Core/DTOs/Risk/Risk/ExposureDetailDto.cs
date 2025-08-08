namespace Core.DTOs.Risk.Risk;

public class ExposureDetailDto
{
    public Guid RiskId { get; set; }
    public string RiskCode { get; set; } = string.Empty;
    public string RiskTitle { get; set; } = string.Empty;
    public decimal Exposure { get; set; }
    public int Probability { get; set; }
    public decimal CostImpact { get; set; }
}