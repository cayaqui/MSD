namespace Core.DTOs.Reports;

public class EarnedValueReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime AsOfDate { get; set; }
    public string ReportCurrency { get; set; } = string.Empty;
    
    // Primary EVM Metrics
    public decimal PlannedValue { get; set; } // PV/BCWS
    public decimal EarnedValue { get; set; } // EV/BCWP
    public decimal ActualCost { get; set; } // AC/ACWP
    public decimal BudgetAtCompletion { get; set; } // BAC
    
    // Variances
    public decimal CostVariance { get; set; } // CV = EV - AC
    public decimal ScheduleVariance { get; set; } // SV = EV - PV
    public decimal VarianceAtCompletion { get; set; } // VAC = BAC - EAC
    
    // Performance Indices
    public decimal CostPerformanceIndex { get; set; } // CPI = EV / AC
    public decimal SchedulePerformanceIndex { get; set; } // SPI = EV / PV
    public decimal ToCompletePerformanceIndex { get; set; } // TCPI = (BAC - EV) / (BAC - AC)
    
    // Forecasts
    public decimal EstimateAtCompletion { get; set; } // EAC
    public decimal EstimateToComplete { get; set; } // ETC = EAC - AC
    public string ForecastMethod { get; set; } = string.Empty; // CPI, SPI*CPI, Custom
    
    // Progress Percentages
    public decimal PercentageComplete { get; set; } // % Complete = EV / BAC
    public decimal PercentageSpent { get; set; } // % Spent = AC / BAC
    public decimal PercentagePlanned { get; set; } // % Planned = PV / BAC
    
    // Time Analysis
    public DateTime ProjectStartDate { get; set; }
    public DateTime ProjectEndDate { get; set; }
    public int PlannedDuration { get; set; } // in days
    public int ActualDuration { get; set; } // in days
    public int ForecastDuration { get; set; } // in days
    public DateTime ForecastCompletionDate { get; set; }
    
    // Control Account Level Details
    public List<ControlAccountEVMReportItemDto> ControlAccounts { get; set; } = new();
    
    // Time-Phased Data (for charts)
    public List<EVMTimePhasedDataDto> TimePhasedData { get; set; } = new();
    
    // Trend Analysis
    public decimal CPITrend { get; set; } // Average CPI over last 3 periods
    public decimal SPITrend { get; set; } // Average SPI over last 3 periods
    public string TrendAnalysis { get; set; } = string.Empty;
    
    // Thresholds and Alerts
    public List<EVMAlertDto> Alerts { get; set; } = new();
    
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class ControlAccountEVMReportItemDto
{
    public string ControlAccountCode { get; set; } = string.Empty;
    public string ControlAccountName { get; set; } = string.Empty;
    public decimal PlannedValue { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal ActualCost { get; set; }
    public decimal BudgetAtCompletion { get; set; }
    public decimal CostVariance { get; set; }
    public decimal ScheduleVariance { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
    public decimal PercentageComplete { get; set; }
    public string CAM { get; set; } = string.Empty; // Control Account Manager
}

public class EVMTimePhasedDataDto
{
    public DateTime Period { get; set; }
    public decimal PlannedValue { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CostVariance { get; set; }
    public decimal ScheduleVariance { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
}

public class EVMAlertDto
{
    public string AlertType { get; set; } = string.Empty; // CostOverrun, ScheduleDelay, Performance
    public string Severity { get; set; } = string.Empty; // High, Medium, Low
    public string Message { get; set; } = string.Empty;
    public string ControlAccountCode { get; set; } = string.Empty;
    public decimal ThresholdValue { get; set; }
    public decimal ActualValue { get; set; }
}