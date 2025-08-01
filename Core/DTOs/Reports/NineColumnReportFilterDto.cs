namespace Core.DTOs.Reports;

/// <summary>
/// DTO for Nine Column Report filters
/// </summary>
public class NineColumnReportFilterDto
{
    public Guid ProjectId { get; set; }
    public DateTime? AsOfDate { get; set; }
    public List<Guid>? PhaseIds { get; set; }
    public List<Guid>? ControlAccountIds { get; set; }
    public bool IncludeWorkPackages { get; set; } = true;
    public bool IncludePlanningPackages { get; set; } = false;
    public bool ShowOnlyActiveItems { get; set; } = true;
    public bool ShowOnlyItemsWithVariance { get; set; } = false;
    public int? MinLevel { get; set; }
    public int? MaxLevel { get; set; }
}