namespace Core.DTOs.Visualization;

public class CashFlowChartDto
{
    public string Title { get; set; } = "Project Cash Flow";
    public List<CashFlowDataPointDto> DataPoints { get; set; } = new();
    public CashFlowSummaryDto Summary { get; set; } = new();
    public CashFlowConfigDto Config { get; set; } = new();
}

public class CashFlowDataPointDto
{
    public DateTime Period { get; set; }
    public decimal PlannedIncome { get; set; }
    public decimal ActualIncome { get; set; }
    public decimal PlannedExpenditure { get; set; }
    public decimal ActualExpenditure { get; set; }
    public decimal NetCashFlow { get; set; }
    public decimal CumulativeCashFlow { get; set; }
}

public class CashFlowSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenditure { get; set; }
    public decimal NetCashFlow { get; set; }
    public decimal MaxCashRequirement { get; set; }
    public DateTime? MaxCashRequirementDate { get; set; }
    public decimal ProjectedFinalBalance { get; set; }
}

public class CashFlowConfigDto
{
    public string PeriodType { get; set; } = "month"; // week, month, quarter
    public bool ShowPlanned { get; set; } = true;
    public bool ShowActual { get; set; } = true;
    public bool ShowCumulative { get; set; } = true;
    public bool ShowNet { get; set; } = true;
}