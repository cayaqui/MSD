using Application.Interfaces.Cost;
using Core.DTOs.EVM;
using Core.DTOs.Reports;
using Web.Models.Responses;

namespace Web.Services.Interfaces;

/// <summary>
/// EVM Service interface for Blazor Web project
/// </summary>
public interface IEVMService
{
    // Nine Column Report operations
    Task<ApiResponse<NineColumnReportDto>> GetNineColumnReportAsync(
        Guid projectId,
        DateTime? asOfDate = null);

    Task<ApiResponse<NineColumnReportDto>> GetFilteredNineColumnReportAsync(
        NineColumnReportFilterDto filter);

    Task<ApiResponse<byte[]>> ExportNineColumnReportToExcelAsync(
        Guid projectId,
        DateTime? asOfDate = null);

    Task<ApiResponse<byte[]>> ExportNineColumnReportToPdfAsync(
        Guid projectId,
        DateTime? asOfDate = null);

    Task<ApiResponse<NineColumnReportDto>> GetNineColumnReportByControlAccountAsync(
        Guid controlAccountId,
        DateTime? asOfDate = null,
        bool includeChildren = true);

    Task<ApiResponse<List<NineColumnReportDto>>> GetNineColumnReportTrendAsync(
        Guid projectId,
        DateTime startDate,
        DateTime endDate,
        string periodType = "Monthly");

    Task<ApiResponse<NineColumnReportValidationResult>> ValidateNineColumnReportAsync(
        Guid projectId,
        DateTime? asOfDate = null);

    // Standard EVM operations
    Task<ApiResponse<List<EVMRecordDto>>> GetEVMRecordsAsync(Guid projectId);
    Task<ApiResponse<EVMRecordDetailDto>> GetEVMRecordAsync(Guid recordId);
    Task<ApiResponse<EVMPerformanceReportDto>> GetProjectEVMReportAsync(Guid projectId);
    Task<ApiResponse<List<EVMTrendDto>>> GetEVMTrendsAsync(Guid controlAccountId);
    Task<ApiResponse<Guid>> CreateEVMRecordAsync(CreateEVMRecordDto dto);
    Task<ApiResponse<bool>> CalculateProjectEVMAsync(Guid projectId);
}