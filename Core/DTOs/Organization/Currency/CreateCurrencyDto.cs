namespace Core.DTOs.Organization.Currency;

/// <summary>
/// DTO for creating a new currency
/// </summary>
public class CreateCurrencyDto
{
    public string Code { get; set; } = string.Empty; // ISO 4217 code
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string? SymbolNative { get; set; }
    
    public int NumericCode { get; set; }
    public int DecimalDigits { get; set; }
    public decimal? Rounding { get; set; }
    
    public string? PluralName { get; set; }
    public string? DecimalSeparator { get; set; } = ".";
    public string? ThousandsSeparator { get; set; } = ",";
    public string? PositivePattern { get; set; }
    public string? NegativePattern { get; set; }
    
    public bool IsBaseCurrency { get; set; }
    public decimal ExchangeRate { get; set; } = 1;
    public string? ExchangeRateSource { get; set; }
    
    public bool IsEnabledForProjects { get; set; } = true;
    public bool IsEnabledForCommitments { get; set; } = true;
}