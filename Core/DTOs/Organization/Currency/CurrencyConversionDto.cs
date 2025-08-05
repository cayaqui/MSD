namespace Core.DTOs.Organization.Currency;

/// <summary>
/// DTO for currency conversion request
/// </summary>
public class CurrencyConversionDto
{
    public decimal Amount { get; set; }
    public string FromCurrencyCode { get; set; } = string.Empty;
    public string ToCurrencyCode { get; set; } = string.Empty;
}