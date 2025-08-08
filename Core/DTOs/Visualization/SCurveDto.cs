namespace Core.DTOs.Visualization;

public class SCurveDto
{
    public string Title { get; set; } = string.Empty;
    public string XAxisLabel { get; set; } = "Time";
    public string YAxisLabel { get; set; } = "Value";
    public List<SCurveSeriesDto> Series { get; set; } = new();
    public SCurveConfigDto Config { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class SCurveSeriesDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "line"; // line, area, bar
    public List<SCurveDataPointDto> DataPoints { get; set; } = new();
    public string Color { get; set; } = string.Empty;
    public bool ShowMarkers { get; set; } = true;
    public string LineStyle { get; set; } = "solid"; // solid, dashed, dotted
    public int LineWidth { get; set; } = 2;
    public bool Fill { get; set; } = false;
}

public class SCurveDataPointDto
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public decimal? CumulativeValue { get; set; }
    public string Label { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class SCurveConfigDto
{
    public string ChartType { get; set; } = "cost"; // cost, progress, resource, cashflow
    public bool ShowLegend { get; set; } = true;
    public bool ShowGrid { get; set; } = true;
    public bool ShowTooltips { get; set; } = true;
    public bool ShowDataLabels { get; set; } = false;
    public bool EnableZoom { get; set; } = true;
    public bool EnablePan { get; set; } = true;
    public string TimeUnit { get; set; } = "month"; // day, week, month, quarter
    public bool ShowCumulative { get; set; } = true;
    public bool ShowVariance { get; set; } = false;
    public string ValueFormat { get; set; } = "currency"; // currency, percentage, number
    public int Height { get; set; } = 400;
}

public class CostSCurveDto : SCurveDto
{
    public decimal TotalBudget { get; set; }
    public decimal CurrentCost { get; set; }
    public decimal ForecastCost { get; set; }
    public DateTime DataDate { get; set; }
}

public class ProgressSCurveDto : SCurveDto
{
    public decimal PlannedProgress { get; set; }
    public decimal ActualProgress { get; set; }
    public decimal ForecastProgress { get; set; }
    public DateTime DataDate { get; set; }
}