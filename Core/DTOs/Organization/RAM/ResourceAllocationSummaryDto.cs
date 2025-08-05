namespace Core.DTOs.Organization.RAM;

/// <summary>
/// DTO for resource allocation summary
/// </summary>
public class ResourceAllocationSummaryDto
{
    public Guid OBSNodeId { get; set; }
    public int TotalAssignments { get; set; }
    public decimal TotalPlannedManHours { get; set; }
    public decimal TotalPlannedCost { get; set; }
    public decimal AverageAllocation { get; set; }
    public List<ResponsibilityTypeSummaryDto> AssignmentsByType { get; set; } = new();
    public List<WBSAllocationSummaryDto> WBSAssignments { get; set; } = new();
}

/// <summary>
/// DTO for responsibility type summary
/// </summary>
public class ResponsibilityTypeSummaryDto
{
    public string ResponsibilityType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAllocation { get; set; }
}

/// <summary>
/// DTO for WBS allocation summary
/// </summary>
public class WBSAllocationSummaryDto
{
    public Guid WBSElementId { get; set; }
    public string WBSCode { get; set; } = string.Empty;
    public string WBSName { get; set; } = string.Empty;
    public string ResponsibilityTypes { get; set; } = string.Empty;
    public decimal TotalAllocation { get; set; }
}