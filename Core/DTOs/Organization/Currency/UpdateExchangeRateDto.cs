namespace Core.DTOs.Organization.Currency;

/// <summary>
/// DTO for updating currency exchange rate
/// </summary>
public class UpdateExchangeRateDto
{
    public decimal Rate { get; set; }
    public string Source { get; set; } = string.Empty;
}