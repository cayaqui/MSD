namespace Core.DTOs.Visualization;

public class ContractStatusChartDto
{
    public string Title { get; set; } = "Contract Status Overview";
    public List<ContractStatusItemDto> Contracts { get; set; } = new();
    public ContractStatusSummaryDto Summary { get; set; } = new();
    public ContractStatusConfigDto Config { get; set; } = new();
}

public class ContractStatusItemDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string ContractorName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal OriginalValue { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal AmountInvoiced { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal PercentageComplete { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysRemaining { get; set; }
    public bool IsDelayed { get; set; }
    public int ChangeOrderCount { get; set; }
    public int OpenClaimsCount { get; set; }
}

public class ContractStatusSummaryDto
{
    public int TotalContracts { get; set; }
    public int ActiveContracts { get; set; }
    public int CompletedContracts { get; set; }
    public int DelayedContracts { get; set; }
    public decimal TotalContractValue { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal AverageProgress { get; set; }
}

public class ContractStatusConfigDto
{
    public bool ShowFinancials { get; set; } = true;
    public bool ShowProgress { get; set; } = true;
    public bool ShowChangeOrders { get; set; } = true;
    public bool ShowClaims { get; set; } = true;
    public string SortBy { get; set; } = "value"; // value, progress, enddate
}