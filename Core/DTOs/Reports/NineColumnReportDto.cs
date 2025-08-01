using System;
using System.Collections.Generic;

namespace Core.DTOs.Reports;

/// <summary>
/// DTO for Nine Column Report (Chilean standard EVM report)
/// Reporte de 9 columnas según estándar chileno
/// </summary>
public class NineColumnReportDto
{
    public Guid ProjectId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public string Currency { get; set; } = "CLP";
    public decimal? ExchangeRate { get; set; } // Para conversión USD/CLP o UF/CLP
    public string PreparedBy { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;

    // Líneas del reporte (Control Accounts y Work Packages)
    public List<NineColumnReportLineDto> Lines { get; set; } = new();

    // Totales del proyecto
    public NineColumnReportTotalsDto Totals { get; set; } = new();

    // Metadata adicional
    public DateTime DataDate { get; set; }
    public string ReportPeriod { get; set; } = string.Empty; // e.g., "Enero 2025"
    public int ReportVersion { get; set; }
}
