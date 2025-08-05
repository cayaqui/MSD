using Core.DTOs.Common;

namespace Core.DTOs.Configuration.SystemParameters;

/// <summary>
/// DTO for System Parameters
/// </summary>
public class SystemParameterDto : BaseDto
{
    public string Category { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string DataType { get; set; } = "String"; // String, Number, Boolean, Date, Json
    public bool IsEncrypted { get; set; }
    public bool IsRequired { get; set; } = true;
    public bool IsSystem { get; set; } // System parameters cannot be deleted
    public bool IsPublic { get; set; } // Can be accessed without authentication
    public string? ValidationRule { get; set; }
    public string? DefaultValue { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public List<string>? AllowedValues { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? LastModifiedBy { get; set; }
}
