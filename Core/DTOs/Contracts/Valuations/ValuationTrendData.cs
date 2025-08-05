namespace Core.DTOs.Contracts.Valuations;

public class ValuationTrendData
{
    public int Period { get; set; }
    public DateTime Date { get; set; }
    public decimal CumulativeValue { get; set; }
    public decimal PeriodValue { get; set; }
    public decimal RetentionAmount { get; set; }
    public decimal NetAmount { get; set; }
}