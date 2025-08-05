namespace Core.DTOs.Organization.Currency;

/// <summary>
/// Currency data transfer object
/// </summary>
public class CurrencyDto
{
    public Guid Id { get; set; }
    
    // Basic Information
    public string Code { get; set; } = string.Empty; // ISO 4217 code
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string? SymbolNative { get; set; }
    
    // Numeric Information
    public int NumericCode { get; set; }
    public int DecimalDigits { get; set; }
    public decimal? Rounding { get; set; }
    
    // Display Information
    public string? PluralName { get; set; }
    public string? DecimalSeparator { get; set; } = ".";
    public string? ThousandsSeparator { get; set; } = ",";
    public string? PositivePattern { get; set; }
    public string? NegativePattern { get; set; }
    
    // Exchange Rate Information
    public bool IsBaseCurrency { get; set; }
    public decimal ExchangeRate { get; set; } = 1;
    public DateTime? ExchangeRateDate { get; set; }
    public string? ExchangeRateSource { get; set; }
    public bool IsExchangeRateStale { get; set; }
    
    // Status
    public bool IsActive { get; set; }
    public bool IsEnabledForProjects { get; set; }
    public bool IsEnabledForCommitments { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}