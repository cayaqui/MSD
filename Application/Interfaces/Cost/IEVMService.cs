
using Core.DTOs.Common;
using Core.DTOs.Cost;
using Core.DTOs.EVM;
using Core.DTOs.Reports;

namespace Application.Interfaces.Cost;

/// <summary>
/// Interface for EVM (Earned Value Management) service
/// </summary>
public interface IEVMService
{
    // Query Operations
    Task<PagedResult<EVMRecordDto>> GetEVMRecordsAsync(
        Guid controlAccountId,
        EVMQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<EVMRecordDetailDto?> GetEVMRecordByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<EVMPerformanceReportDto> GetProjectEVMReportAsync(
        Guid projectId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default);

    Task<List<EVMTrendDto>> GetEVMTrendsAsync(
        Guid controlAccountId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    // Command Operations
    Task<Result<Guid>> CreateEVMRecordAsync(
        CreateEVMRecordDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateEVMActualsAsync(
        Guid id,
        UpdateEVMActualsDto dto,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> CalculateProjectEVMAsync(
        Guid projectId,
        DateTime dataDate,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> GenerateMonthlyEVMAsync(
        Guid projectId,
        int year,
        int month,
        string userId,
        CancellationToken cancellationToken = default);

    // Forecast Operations
    Task<Result> UpdateEACAsync(
        Guid evmRecordId,
        decimal newEAC,
        string justification,
        string userId,
        CancellationToken cancellationToken = default);
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
