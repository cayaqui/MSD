namespace Core.DTOs.Visualization;

public class EVMChartDto
{
    public string Title { get; set; } = "Earned Value Management";
    public List<EVMDataPointDto> DataPoints { get; set; } = new();
    public EVMMetricsDto CurrentMetrics { get; set; } = new();
    public EVMConfigDto Config { get; set; } = new();
}

public class EVMDataPointDto
{
    public DateTime Date { get; set; }
    public decimal PlannedValue { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal ActualCost { get; set; }
    public decimal BudgetAtCompletion { get; set; }
    public decimal? EstimateAtCompletion { get; set; }
    public decimal? EstimateToComplete { get; set; }
}

public class EVMMetricsDto
{
    public decimal CostVariance { get; set; }
    public decimal ScheduleVariance { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
    public decimal EstimateAtCompletion { get; set; }
    public decimal EstimateToComplete { get; set; }
    public decimal VarianceAtCompletion { get; set; }
    public decimal ToCompletePerformanceIndex { get; set; }
}

public class EVMConfigDto
{
    public bool ShowPlannedValue { get; set; } = true;
    public bool ShowEarnedValue { get; set; } = true;
    public bool ShowActualCost { get; set; } = true;
    public bool ShowEstimates { get; set; } = true;
    public bool ShowVariances { get; set; } = true;
    public string DateFormat { get; set; } = "MMM dd";
}