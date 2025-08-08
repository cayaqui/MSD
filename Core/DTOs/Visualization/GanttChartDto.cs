namespace Core.DTOs.Visualization;

public class GanttChartDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<GanttTaskDto> Tasks { get; set; } = new();
    public List<GanttMilestoneDto> Milestones { get; set; } = new();
    public List<string> CriticalPath { get; set; } = new();
    public GanttConfigDto Config { get; set; } = new();
}

public class GanttTaskDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ParentId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Progress { get; set; }
    public string AssignedTo { get; set; } = string.Empty;
    public string Type { get; set; } = "task"; // task, summary, milestone
    public bool IsCritical { get; set; }
    public List<string> Dependencies { get; set; } = new();
    public string Color { get; set; } = string.Empty;
    public Dictionary<string, object> CustomData { get; set; } = new();
}

public class GanttMilestoneDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class GanttConfigDto
{
    public string ViewMode { get; set; } = "month"; // day, week, month, quarter, year
    public bool ShowDependencies { get; set; } = true;
    public bool ShowProgress { get; set; } = true;
    public bool ShowCriticalPath { get; set; } = true;
    public bool ShowMilestones { get; set; } = true;
    public bool ShowWeekends { get; set; } = false;
    public bool ShowToday { get; set; } = true;
    public int RowHeight { get; set; } = 30;
    public int HeaderHeight { get; set; } = 50;
    public string DateFormat { get; set; } = "MMM dd";
}