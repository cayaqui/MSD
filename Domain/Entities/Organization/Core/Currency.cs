using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Organization.Core;

/// <summary>
/// Currency entity for multi-currency support
/// </summary>
public class Currency : BaseEntity, ICodeEntity, INamedEntity, IActivatable
{
    // Basic Information
    public string Code { get; set; } = string.Empty; // ISO 4217 code (USD, EUR, CLP, etc.)
    public string Name { get; set; } = string.Empty; // US Dollar, Euro, Chilean Peso
    public string Symbol { get; set; } = string.Empty; // $, €, etc.
    public string? SymbolNative { get; set; } // Native symbol if different

    // Numeric Information
    public int NumericCode { get; set; } // ISO 4217 numeric code
    public int DecimalDigits { get; set; } // Number of decimal places
    public decimal? Rounding { get; set; } // Rounding precision

    // Display Information
    public string? PluralName { get; set; } // Dollars, Euros, Pesos
    public string? DecimalSeparator { get; set; } = ".";
    public string? ThousandsSeparator { get; set; } = ",";
    public string? PositivePattern { get; set; } // e.g., "$n" or "n $"
    public string? NegativePattern { get; set; } // e.g., "($n)" or "-$n"

    // Exchange Rate Information
    public bool IsBaseCurrency { get; set; }
    public decimal ExchangeRate { get; set; } = 1; // Rate to base currency
    public DateTime? ExchangeRateDate { get; set; }
    public string? ExchangeRateSource { get; set; }

    // Status
    public bool IsActive { get; set; }
    public bool IsEnabledForProjects { get; set; }
    public bool IsEnabledForCommitments { get; set; }

    // Navigation Properties
    public ICollection<Project> Projects { get; set; } = new List<Project>();

    // Constructor for EF Core
    private Currency() { }

    public Currency(
        string code,
        string name,
        string symbol,
        int numericCode,
        int decimalDigits)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        NumericCode = numericCode;
        DecimalDigits = decimalDigits;
        ExchangeRate = 1;
        IsActive = true;
        IsEnabledForProjects = true;
        IsEnabledForCommitments = true;
    }

    // Domain Methods
    public void UpdateExchangeRate(decimal rate, string source, string updatedBy)
    {
        if (IsBaseCurrency)
            throw new InvalidOperationException("Cannot update exchange rate for base currency");

        if (rate <= 0)
            throw new ArgumentException("Exchange rate must be greater than zero");

        ExchangeRate = rate;
        ExchangeRateDate = DateTime.UtcNow;
        ExchangeRateSource = source;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsBaseCurrency(string updatedBy)
    {
        IsBaseCurrency = true;
        ExchangeRate = 1;
        ExchangeRateDate = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDisplaySettings(
        string? decimalSeparator,
        string? thousandsSeparator,
        string? positivePattern,
        string? negativePattern,
        string updatedBy)
    {
        DecimalSeparator = decimalSeparator ?? ".";
        ThousandsSeparator = thousandsSeparator ?? ",";
        PositivePattern = positivePattern;
        NegativePattern = negativePattern;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableForProjects(bool enable, string updatedBy)
    {
        IsEnabledForProjects = enable;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableForCommitments(bool enable, string updatedBy)
    {
        IsEnabledForCommitments = enable;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    // Conversion Methods
    public decimal ConvertToBaseCurrency(decimal amount)
    {
        if (IsBaseCurrency) return amount;
        return amount * ExchangeRate;
    }

    public decimal ConvertFromBaseCurrency(decimal amount)
    {
        if (IsBaseCurrency) return amount;
        if (ExchangeRate == 0) throw new InvalidOperationException("Exchange rate cannot be zero");
        return amount / ExchangeRate;
    }

    public decimal ConvertTo(decimal amount, Currency targetCurrency)
    {
        if (targetCurrency == null)
            throw new ArgumentNullException(nameof(targetCurrency));

        if (Code == targetCurrency.Code)
            return amount;

        // Convert to base currency first, then to target
        var baseAmount = ConvertToBaseCurrency(amount);
        return targetCurrency.ConvertFromBaseCurrency(baseAmount);
    }

    // Formatting
    public string FormatAmount(decimal amount)
    {
        var formattedNumber = amount.ToString($"N{DecimalDigits}");

        // Apply thousand and decimal separators
        if (DecimalSeparator != ".")
            formattedNumber = formattedNumber.Replace(".", DecimalSeparator);
        if (ThousandsSeparator != ",")
            formattedNumber = formattedNumber.Replace(",", ThousandsSeparator);

        // Apply positive/negative pattern
        if (amount >= 0 && !string.IsNullOrEmpty(PositivePattern))
        {
            return PositivePattern.Replace("n", formattedNumber);
        }
        else if (amount < 0 && !string.IsNullOrEmpty(NegativePattern))
        {
            formattedNumber = formattedNumber.TrimStart('-');
            return NegativePattern.Replace("n", formattedNumber);
        }

        // Default format
        return $"{Symbol}{formattedNumber}";
    }

    public bool IsExchangeRateStale(int maxDays = 7)
    {
        if (IsBaseCurrency) return false;
        if (!ExchangeRateDate.HasValue) return true;
        return (DateTime.UtcNow - ExchangeRateDate.Value).TotalDays > maxDays;
    }
}