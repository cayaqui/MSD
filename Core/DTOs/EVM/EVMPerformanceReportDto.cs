using System;
using System.Collections.Generic;

namespace Core.DTOs.EVM;

/// <summary>
/// DTO for EVM performance report
/// </summary>
public class EVMPerformanceReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public decimal TotalBAC { get; set; }
    public decimal TotalPV { get; set; }
    public decimal TotalEV { get; set; }
    public decimal TotalAC { get; set; }
    public decimal TotalCV { get; set; }
    public decimal TotalSV { get; set; }
    public decimal OverallCPI { get; set; }
    public decimal OverallSPI { get; set; }
    public decimal ProjectEAC { get; set; }
    public decimal ProjectVAC { get; set; }
    public decimal ProjectPercentComplete { get; set; }
    public List<ControlAccountEVMDto> ControlAccounts { get; set; } = new();
    public List<EVMTrendDto> Trends { get; set; } = new();
}
