namespace Core.DTOs.Organization.Currency;

/// <summary>
/// DTO for updating currency information
/// </summary>
public class UpdateCurrencyDto
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string? SymbolNative { get; set; }
    
    public string? PluralName { get; set; }
    public string? DecimalSeparator { get; set; } = ".";
    public string? ThousandsSeparator { get; set; } = ",";
    public string? PositivePattern { get; set; }
    public string? NegativePattern { get; set; }
    
    public bool IsActive { get; set; }
    public bool IsEnabledForProjects { get; set; }
    public bool IsEnabledForCommitments { get; set; }
}