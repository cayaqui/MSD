using Core.Enums.Cost;
using System;
using System.Collections.Generic;

namespace Core.DTOs.Cost;

/// <summary>
/// DTO for Cost Trend data
/// </summary>
public class CostTrendDto
{
    public DateTime Date { get; set; }
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CommittedCost { get; set; }
    public decimal ForecastCost { get; set; }
}
// Cost Entries
public class CostEntryDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public CostType CostType { get; set; }
    public Guid ControlAccountId { get; set; }
    public string ControlAccountCode { get; set; } = string.Empty;
    public string ControlAccountName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Vendor { get; set; }
    public string? Reference { get; set; }
    public decimal Amount { get; set; }
    public CostStatus Status { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? Notes { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceFrequency { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateCostEntryDto
{
    public Guid ProjectId { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public CostType CostType { get; set; }
    public Guid ControlAccountId { get; set; }
    public Guid CategoryId { get; set; }
    public string? Vendor { get; set; }
    public string? Reference { get; set; }
    public decimal Amount { get; set; }
    public CostStatus Status { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? Notes { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceFrequency { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
}

public class UpdateCostEntryDto
{
    public DateTime? Date { get; set; }
    public string? Description { get; set; }
    public CostType? CostType { get; set; }
    public Guid? ControlAccountId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Vendor { get; set; }
    public string? Reference { get; set; }
    public decimal? Amount { get; set; }
    public CostStatus? Status { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? Notes { get; set; }
    public bool? IsRecurring { get; set; }
    public string? RecurrenceFrequency { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
}

// Summary and Analysis
public class CostSummaryDto
{
    public Guid ProjectId { get; set; }
    public decimal BAC { get; set; } // Budget at Completion
    public decimal AC { get; set; } // Actual Cost
    public decimal EV { get; set; } // Earned Value
    public decimal PV { get; set; } // Planned Value
    public decimal CommittedCost { get; set; }
    public decimal RemainingBudget { get; set; }
    public decimal EAC { get; set; } // Estimate at Completion
    public decimal ETC { get; set; } // Estimate to Complete
    public decimal VAC { get; set; } // Variance at Completion
    public decimal CPI { get; set; } // Cost Performance Index
    public decimal TCPI { get; set; } // To-Complete Performance Index
    public DateTime LastUpdated { get; set; }
}

public class CostCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal Actual { get; set; }
    public decimal Committed { get; set; }
    public int TransactionCount { get; set; }
}

public class CostAnalysisDto
{
    public Dictionary<string, decimal> CostByCategory { get; set; } = new();
    public Dictionary<string, decimal> CostByMonth { get; set; } = new();
    public Dictionary<string, decimal> CostByControlAccount { get; set; } = new();
    public List<CostTrendDto> Trends { get; set; } = new();
    public List<CostVarianceDto> Variances { get; set; } = new();
}

public class CostVarianceDto
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal Actual { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercentage { get; set; }
}

public class CashFlowDto
{
    public string Period { get; set; } = string.Empty;
    public decimal PlannedInflow { get; set; }
    public decimal PlannedOutflow { get; set; }
    public decimal ActualInflow { get; set; }
    public decimal ActualOutflow { get; set; }
    public decimal ProjectedInflow { get; set; }
    public decimal ProjectedOutflow { get; set; }
    public decimal CumulativeBalance { get; set; }
}

// Filtros
public class CostFilterDto
{
    public Guid ProjectId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public CostType? CostType { get; set; }
    public Guid? ControlAccountId { get; set; }
    public Guid? CategoryId { get; set; }
    public CostStatus? Status { get; set; }
    public string? Vendor { get; set; }
    public decimal? AmountMin { get; set; }
    public decimal? AmountMax { get; set; }
    public string? SearchTerm { get; set; }
}