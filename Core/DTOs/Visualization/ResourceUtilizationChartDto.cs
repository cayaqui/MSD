namespace Core.DTOs.Visualization;

public class ResourceUtilizationChartDto
{
    public string Title { get; set; } = "Resource Utilization";
    public List<ResourceSeriesDto> Series { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public ResourceUtilizationConfigDto Config { get; set; } = new();
}

public class ResourceSeriesDto
{
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public List<ResourceDataPointDto> DataPoints { get; set; } = new();
    public string Color { get; set; } = string.Empty;
}

public class ResourceDataPointDto
{
    public DateTime Date { get; set; }
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal Capacity { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public bool IsOverAllocated => ActualHours > Capacity;
}

public class ResourceUtilizationConfigDto
{
    public string ViewMode { get; set; } = "week"; // day, week, month
    public bool ShowPlanned { get; set; } = true;
    public bool ShowActual { get; set; } = true;
    public bool ShowCapacity { get; set; } = true;
    public bool StackResources { get; set; } = false;
}