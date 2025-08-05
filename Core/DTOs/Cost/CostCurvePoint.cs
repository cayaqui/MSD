namespace Core.DTOs.Cost;

public class CostCurvePoint
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string? Label { get; set; }
}