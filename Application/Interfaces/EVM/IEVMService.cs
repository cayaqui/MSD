using System;
using System.Threading;
using System.Threading.Tasks;
using Core.DTOs.Reports;
using Core.DTOs.EVM;
using Core.DTOs.Common;
using Domain.Common;

namespace Application.Interfaces.Cost;

/// <summary>
/// Extended interface for EVM Service including Nine Column Report
/// This is an extension to the existing IEVMService interface
/// </summary>
public partial interface IEVMService
{
    // Nine Column Report Operations

    /// <summary>
    /// Get Nine Column Report for a project
    /// Obtiene el reporte de 9 columnas según estándar chileno
    /// </summary>
    Task<NineColumnReportDto> GetNineColumnReportAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get filtered Nine Column Report
    /// Obtiene el reporte de 9 columnas con filtros aplicados
    /// </summary>
    Task<NineColumnReportDto> GetFilteredNineColumnReportAsync(
        NineColumnReportFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Export Nine Column Report to Excel
    /// Exporta el reporte de 9 columnas a Excel
    /// </summary>
    Task<byte[]> ExportNineColumnReportToExcelAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Export Nine Column Report to PDF
    /// Exporta el reporte de 9 columnas a PDF
    /// </summary>
    Task<byte[]> ExportNineColumnReportToPdfAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Nine Column Report by Control Account
    /// Obtiene el reporte para un Control Account específico y sus hijos
    /// </summary>
    Task<NineColumnReportDto> GetNineColumnReportByControlAccountAsync(
        Guid controlAccountId,
        DateTime? asOfDate = null,
        bool includeChildren = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Nine Column Report trend data
    /// Obtiene datos históricos del reporte para análisis de tendencias
    /// </summary>
    Task<List<NineColumnReportDto>> GetNineColumnReportTrendAsync(
        Guid projectId,
        DateTime startDate,
        DateTime endDate,
        string periodType = "Monthly", // Daily, Weekly, Monthly
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate Nine Column Report data
    /// Valida la integridad de los datos del reporte
    /// </summary>
    Task<Result<NineColumnReportValidationResult>> ValidateNineColumnReportDataAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default);
}
