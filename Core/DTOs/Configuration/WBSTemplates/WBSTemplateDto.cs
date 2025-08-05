using Core.DTOs.Common;

namespace Core.DTOs.Configuration.WBSTemplates;

/// <summary>
/// DTO for WBS Template configuration
/// </summary>
public class WBSTemplateDto : BaseDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IndustryType { get; set; } = string.Empty; // Oil&Gas, Construction, IT, etc.
    public string ProjectType { get; set; } = string.Empty;
    
    // Template structure
    public List<WBSTemplateElementDto> Elements { get; set; } = new();
    public int TotalElements { get; set; }
    public int MaxLevel { get; set; }
    
    // Settings
    public string CodingScheme { get; set; } = "Numeric"; // Numeric, Alpha, AlphaNumeric
    public string Delimiter { get; set; } = ".";
    public int CodeLength { get; set; } = 3;
    public bool AutoGenerateCodes { get; set; } = true;
    
    // Usage
    public bool IsPublic { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public int UsageCount { get; set; }
    public DateTime? LastUsedDate { get; set; }
}

public class WBSTemplateElementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public int Level { get; set; }
    public int SequenceNumber { get; set; }
    public string ElementType { get; set; } = string.Empty;
    public bool IsOptional { get; set; }
    public List<WBSTemplateElementDto> Children { get; set; } = new();
    
    // Default values
    public decimal? DefaultBudgetPercentage { get; set; }
    public int? DefaultDurationDays { get; set; }
    public string? DefaultDiscipline { get; set; }
}

public class CreateWBSTemplateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IndustryType { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public string CodingScheme { get; set; } = "Numeric";
    public string Delimiter { get; set; } = ".";
    public int CodeLength { get; set; } = 3;
    public bool AutoGenerateCodes { get; set; } = true;
    public bool IsPublic { get; set; } = true;
}

public class UpdateWBSTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IndustryType { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}

public class ImportWBSTemplateDto
{
    public string Format { get; set; } = "Excel"; // Excel, XML, JSON
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public bool ValidateOnly { get; set; } = false;
    public bool MergeWithExisting { get; set; } = false;
}

/// <summary>
/// DTO for CBS Template configuration
/// </summary>
public class CBSTemplateDto : BaseDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IndustryType { get; set; } = string.Empty;
    public string CostType { get; set; } = "All"; // Labor, Material, Equipment, Subcontract, All
    
    // Template structure
    public List<CBSTemplateElementDto> Elements { get; set; } = new();
    public int TotalElements { get; set; }
    public int MaxLevel { get; set; }
    
    // Settings
    public string CodingScheme { get; set; } = "Numeric";
    public string Delimiter { get; set; } = ".";
    public bool IncludesIndirectCosts { get; set; } = true;
    public bool IncludesContingency { get; set; } = true;
    
    // Usage
    public bool IsPublic { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public int UsageCount { get; set; }
}

public class CBSTemplateElementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public int Level { get; set; }
    public string CostType { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal? UnitRate { get; set; }
    public bool IsControlAccount { get; set; }
    public List<CBSTemplateElementDto> Children { get; set; } = new();
}

public class CreateCBSTemplateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IndustryType { get; set; } = string.Empty;
    public string CostType { get; set; } = "All";
    public string CodingScheme { get; set; } = "Numeric";
    public string Delimiter { get; set; } = ".";
    public bool IncludesIndirectCosts { get; set; } = true;
    public bool IncludesContingency { get; set; } = true;
    public bool IsPublic { get; set; } = true;
}

public class UpdateCBSTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string IndustryType { get; set; } = string.Empty;
    public string CostType { get; set; } = "All";
    public bool IncludesIndirectCosts { get; set; }
    public bool IncludesContingency { get; set; }
    public bool IsPublic { get; set; }
}

public class WBSCBSTemplateFilterDto
{
    public string? Type { get; set; } // WBS or CBS
    public string? IndustryType { get; set; }
    public string? ProjectType { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class TemplateValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public int ElementsValidated { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
}

public class TemplateUsageStatisticsDto
{
    public Guid TemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty; // WBS or CBS
    public int TotalUsageCount { get; set; }
    public int ActiveProjectsCount { get; set; }
    public int CompletedProjectsCount { get; set; }
    public DateTime? LastUsedDate { get; set; }
    public decimal AverageProjectValue { get; set; }
    public List<string> TopIndustries { get; set; } = new();
    public List<MonthlyUsageDto> MonthlyUsage { get; set; } = new();
}

public class MonthlyUsageDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int UsageCount { get; set; }
}