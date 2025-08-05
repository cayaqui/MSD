namespace Core.DTOs.Reports;

public class CostControlReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string ReportCurrency { get; set; } = string.Empty;
    
    // Budget Information
    public decimal OriginalBudget { get; set; }
    public decimal ApprovedChanges { get; set; }
    public decimal CurrentBudget { get; set; }
    
    // Cost Performance
    public decimal ActualCostToDate { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public decimal EstimateAtCompletion { get; set; }
    public decimal EstimateToComplete { get; set; }
    public decimal VarianceAtCompletion { get; set; }
    
    // Period Costs
    public decimal PeriodActualCost { get; set; }
    public decimal PeriodPlannedCost { get; set; }
    public decimal PeriodVariance { get; set; }
    
    // Control Accounts
    public List<ControlAccountCostDto> ControlAccounts { get; set; } = new();
    
    // Cost Categories
    public List<CostCategoryDto> CostCategories { get; set; } = new();
    
    // Commitments
    public decimal TotalCommitments { get; set; }
    public decimal CommitmentsThisPeriod { get; set; }
    public List<CommitmentReportItemDto> MajorCommitments { get; set; } = new();
    
    // Change Orders
    public int ApprovedChangeOrders { get; set; }
    public int PendingChangeOrders { get; set; }
    public decimal ApprovedChangeOrderValue { get; set; }
    public decimal PendingChangeOrderValue { get; set; }
    
    // Forecasting
    public string ForecastMethod { get; set; } = string.Empty;
    public decimal ForecastAccuracy { get; set; }
    public string ForecastNotes { get; set; } = string.Empty;
    
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class ControlAccountCostDto
{
    public string ControlAccountCode { get; set; } = string.Empty;
    public string ControlAccountName { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
    public decimal Variance { get; set; }
    public decimal PercentageComplete { get; set; }
    public string CAM { get; set; } = string.Empty; // Control Account Manager
}

public class CostCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal CommittedAmount { get; set; }
    public decimal VarianceAmount { get; set; }
    public decimal PercentageOfTotal { get; set; }
}

public class CommitmentReportItemDto
{
    public string CommitmentNumber { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal SpentToDate { get; set; }
    public decimal RemainingAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}