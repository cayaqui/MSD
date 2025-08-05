namespace Core.DTOs.Cost.ExchangeRate;

public class BulkUpdateExchangeRatesDto
{
    public List<ExchangeRateUpdate> Updates { get; set; } = new();
}
