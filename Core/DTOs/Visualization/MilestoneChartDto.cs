namespace Core.DTOs.Visualization;

public class MilestoneChartDto
{
    public string Title { get; set; } = "Project Milestones";
    public List<MilestoneGroupDto> Groups { get; set; } = new();
    public MilestoneChartConfigDto Config { get; set; } = new();
}

public class MilestoneGroupDto
{
    public string GroupName { get; set; } = string.Empty;
    public List<MilestoneItemDto> Milestones { get; set; } = new();
}

public class MilestoneItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsCritical { get; set; }
    public int DaysVariance { get; set; }
    public string ResponsibleParty { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class MilestoneChartConfigDto
{
    public bool ShowPastMilestones { get; set; } = true;
    public bool ShowFutureMilestones { get; set; } = true;
    public bool GroupByType { get; set; } = true;
    public bool ShowVariance { get; set; } = true;
    public string TimelineView { get; set; } = "months"; // days, weeks, months, quarters
}