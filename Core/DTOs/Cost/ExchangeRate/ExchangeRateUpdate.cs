namespace Core.DTOs.Cost.ExchangeRate;

public class ExchangeRateUpdate
{
    public Guid CurrencyId { get; set; }
    public decimal ExchangeRate { get; set; }
}
