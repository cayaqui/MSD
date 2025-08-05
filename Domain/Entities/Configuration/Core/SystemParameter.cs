using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Domain.Entities.Configuration.Core;

/// <summary>
/// Represents a system configuration parameter
/// </summary>
public class SystemParameter : BaseAuditableEntity
{
    public string Category { get; private set; } = string.Empty;
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public string? Description { get; private set; }
    public string DataType { get; private set; } = "String";
    public bool IsEncrypted { get; private set; }
    public bool IsRequired { get; private set; } = true;
    public bool IsSystem { get; private set; } // System parameters cannot be deleted
    public bool IsPublic { get; private set; } // Can be accessed without authentication

    // Validation
    public string? ValidationRule { get; private set; }
    public string? DefaultValue { get; private set; }
    public int? MinValue { get; private set; }
    public int? MaxValue { get; private set; }
    public string? AllowedValuesJson { get; private set; }

    // Audit
    public DateTime? LastModifiedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }

    private SystemParameter() { } // EF Core

    public SystemParameter(
        string category,
        string key,
        string value,
        string dataType,
        bool isSystem = false
    )
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
        IsSystem = isSystem;
    }

    public void UpdateValue(string value, string? modifiedBy = null)
    {
        ValidateValue(value);
        Value = value ?? throw new ArgumentNullException(nameof(value));
        LastModifiedDate = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void UpdateMetadata(
        string? displayName,
        string? description,
        bool isRequired,
        bool isPublic
    )
    {
        DisplayName = displayName;
        Description = description;
        IsRequired = isRequired;
        IsPublic = isPublic;
    }

    public void SetValidationRules(
        string? validationRule,
        int? minValue,
        int? maxValue,
        List<string>? allowedValues
    )
    {
        ValidationRule = validationRule;
        MinValue = minValue;
        MaxValue = maxValue;
        SetAllowedValues(allowedValues);
    }

    public void SetAllowedValues(List<string>? values)
    {
        AllowedValuesJson = values?.Any() == true ? JsonSerializer.Serialize(values) : null;
    }

    public List<string> GetAllowedValues()
    {
        return string.IsNullOrEmpty(AllowedValuesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(AllowedValuesJson) ?? new List<string>();
    }

    public void SetEncryption(bool isEncrypted)
    {
        IsEncrypted = isEncrypted;
    }

    public void SetDefaultValue(string? defaultValue)
    {
        if (!string.IsNullOrEmpty(defaultValue))
        {
            ValidateValue(defaultValue);
        }
        DefaultValue = defaultValue;
    }

    private void ValidateValue(string value)
    {
        if (string.IsNullOrEmpty(value) && IsRequired)
            throw new InvalidOperationException($"Parameter {Key} is required and cannot be empty");

        switch (DataType.ToLower())
        {
            case "number":
                if (!int.TryParse(value, out var numValue))
                    throw new InvalidOperationException($"Value '{value}' is not a valid number");

                if (MinValue.HasValue && numValue < MinValue)
                    throw new InvalidOperationException($"Value must be at least {MinValue}");

                if (MaxValue.HasValue && numValue > MaxValue)
                    throw new InvalidOperationException($"Value must not exceed {MaxValue}");
                break;

            case "boolean":
                if (!bool.TryParse(value, out _))
                    throw new InvalidOperationException($"Value '{value}' is not a valid boolean");
                break;

            case "date":
                if (!DateTime.TryParse(value, out _))
                    throw new InvalidOperationException($"Value '{value}' is not a valid date");
                break;

            case "json":
                try
                {
                    JsonDocument.Parse(value);
                }
                catch
                {
                    throw new InvalidOperationException($"Value is not valid JSON");
                }
                break;
        }

        // Check allowed values
        var allowedValues = GetAllowedValues();
        if (allowedValues.Any() && !allowedValues.Contains(value))
        {
            throw new InvalidOperationException(
                $"Value '{value}' is not in the list of allowed values"
            );
        }
    }

    public T GetValueAs<T>()
    {
        try
        {
            return DataType.ToLower() switch
            {
                "json" => JsonSerializer.Deserialize<T>(Value)!,
                _ => (T)Convert.ChangeType(Value, typeof(T)),
            };
        }
        catch
        {
            throw new InvalidOperationException(
                $"Cannot convert parameter value to type {typeof(T).Name}"
            );
        }
    }

    public bool TryGetValueAs<T>(out T? result)
    {
        try
        {
            result = GetValueAs<T>();
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
