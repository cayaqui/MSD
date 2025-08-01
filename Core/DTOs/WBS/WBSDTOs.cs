using Core.Enums.Projects;
using Core.Enums.Progress;
using Core.Enums.Cost;

namespace Core.DTOs.WBS;

/// <summary>
/// DTO for WBS Element list view
/// </summary>
public class WBSElementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ParentId { get; set; }
    public int Level { get; set; }
    public int SequenceNumber { get; set; }
    public string FullPath { get; set; } = string.Empty;
    public WBSElementType ElementType { get; set; }
    public Guid? ControlAccountId { get; set; }
    public string? ControlAccountCode { get; set; }
    public bool IsActive { get; set; }
    public int ChildrenCount { get; set; }
    public bool CanHaveChildren { get; set; }
    public decimal? ProgressPercentage { get; set; }
    public WorkPackageStatus? Status { get; set; }
}

/// <summary>
/// DTO for WBS Element detailed view
/// </summary>
public class WBSElementDetailDto : WBSElementDto
{
    public WBSDictionaryDto? Dictionary { get; set; }
    public WorkPackageDetailsDto? WorkPackageDetails { get; set; }
    public List<WBSElementDto> Children { get; set; } = [];
    public List<WBSCBSMappingDto> CBSMappings { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// DTO for WBS Dictionary information
/// </summary>
public class WBSDictionaryDto
{
    public Guid WBSElementId { get; set; }
    public string? DeliverableDescription { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string? Assumptions { get; set; }
    public string? Constraints { get; set; }
    public string? ExclusionsInclusions { get; set; }
}

/// <summary>
/// DTO for Work Package specific details
/// </summary>
public class WorkPackageDetailsDto
{
    public Guid WBSElementId { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? BaselineStartDate { get; set; }
    public DateTime? BaselineEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public DateTime? ForecastStartDate { get; set; }
    public DateTime? ForecastEndDate { get; set; }
    public int PlannedDuration { get; set; }
    public int? ActualDuration { get; set; }
    public int? RemainingDuration { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal ProgressPercentage { get; set; }
    public decimal PhysicalProgressPercentage { get; set; }
    public ProgressMethod ProgressMethod { get; set; }
    public WorkPackageStatus Status { get; set; }
    public decimal? WeightFactor { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public string? ResponsibleUserName { get; set; }
    public Guid? PrimaryDisciplineId { get; set; }
    public string? PrimaryDisciplineName { get; set; }
    public decimal? CPI { get; set; }
    public decimal? SPI { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal PlannedValue { get; set; }
    public bool IsBaselined { get; set; }
    public DateTime? BaselineDate { get; set; }
    public bool IsCriticalPath { get; set; }
    public decimal TotalFloat { get; set; }
    public decimal FreeFloat { get; set; }
}

/// <summary>
/// DTO for WBS-CBS mapping
/// </summary>
public class WBSCBSMappingDto
{
    public Guid Id { get; set; }
    public Guid WBSElementId { get; set; }
    public Guid CBSId { get; set; }
    public string CBSCode { get; set; } = string.Empty;
    public string CBSName { get; set; } = string.Empty;
    public decimal AllocationPercentage { get; set; }
    public bool IsPrimary { get; set; }
    public string? AllocationBasis { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// DTO for creating WBS Element
/// </summary>
public class CreateWBSElementDto
{
    public Guid ProjectId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public WBSElementType ElementType { get; set; }
    public int? SequenceNumber { get; set; }
}

/// <summary>
/// DTO for updating WBS Element
/// </summary>
public class UpdateWBSElementDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating WBS Dictionary
/// </summary>
public class UpdateWBSDictionaryDto
{
    public string? DeliverableDescription { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string? Assumptions { get; set; }
    public string? Constraints { get; set; }
    public string? ExclusionsInclusions { get; set; }
}

/// <summary>
/// DTO for converting to Work Package
/// </summary>
public class ConvertToWorkPackageDto
{
    public Guid ControlAccountId { get; set; }
    public ProgressMethod ProgressMethod { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public decimal Budget { get; set; }
    public string Currency { get; set; } = "USD";
    public Guid? ResponsibleUserId { get; set; }
    public Guid? PrimaryDisciplineId { get; set; }
    public decimal? WeightFactor { get; set; }
}

/// <summary>
/// DTO for converting Planning Package to Work Package
/// </summary>
public class ConvertPlanningToWorkPackageDto
{
    public ProgressMethod ProgressMethod { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public decimal Budget { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public Guid? PrimaryDisciplineId { get; set; }
    public decimal? WeightFactor { get; set; }
}

/// <summary>
/// DTO for reordering WBS Elements
/// </summary>
public class ReorderWBSElementsDto
{
    public Guid ParentId { get; set; }
    public List<WBSElementOrderDto> Elements { get; set; } = new();
}

/// <summary>
/// DTO for WBS Element ordering
/// </summary>
public class WBSElementOrderDto
{
    public Guid Id { get; set; }
    public int SequenceNumber { get; set; }
}

/// <summary>
/// Result of WBS code validation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Suggestions { get; set; } = new();
}