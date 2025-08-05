namespace Core.DTOs.Cost.ExchangeRate;

public class ExchangeRateHistoryDto
{
    public DateTime Date { get; set; }
    public decimal Rate { get; set; }
    public string? UpdatedBy { get; set; }
    public string? Notes { get; set; }
}