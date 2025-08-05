namespace Core.DTOs.Organization.Currency;

/// <summary>
/// DTO for currency conversion result
/// </summary>
public class CurrencyConversionResultDto
{
    public decimal OriginalAmount { get; set; }
    public string FromCurrency { get; set; } = string.Empty;
    public decimal ConvertedAmount { get; set; }
    public string ToCurrency { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public DateTime ConversionDate { get; set; }
}