using Core.DTOs.Common;
using Core.DTOs.EVM;
using Core.DTOs.Reports;

namespace Web.Services.Interfaces.Cost;

public interface IEVMApiService
{
    // Query operations
    Task<PagedResult<EVMRecordDto>> GetEVMRecordsAsync(Guid controlAccountId, EVMQueryParameters parameters);
    Task<EVMRecordDetailDto?> GetEVMRecordByIdAsync(Guid id);
    Task<EVMPerformanceReportDto?> GetProjectEVMReportAsync(Guid projectId, DateTime? asOfDate = null);
    Task<List<EVMTrendDto>> GetEVMTrendsAsync(Guid controlAccountId, DateTime startDate, DateTime endDate);
    
    // Command operations
    Task<Guid?> CreateEVMRecordAsync(CreateEVMRecordDto dto);
    Task<bool> UpdateEVMActualsAsync(Guid id, UpdateEVMActualsDto dto);
    Task<bool> CalculateProjectEVMAsync(Guid projectId, DateTime dataDate);
    Task<bool> GenerateMonthlyEVMAsync(Guid projectId, int year, int month);
    
    // Forecast operations
    Task<bool> UpdateEACAsync(Guid id, decimal newEAC, string justification);
    
    // Nine Column Report operations
    Task<NineColumnReportDto?> GetNineColumnReportAsync(Guid projectId, DateTime? asOfDate = null);
    Task<NineColumnReportDto?> GetFilteredNineColumnReportAsync(NineColumnReportFilterDto filter);
    Task<byte[]> ExportNineColumnReportToExcelAsync(Guid projectId, DateTime? asOfDate = null);
    Task<byte[]> ExportNineColumnReportToPdfAsync(Guid projectId, DateTime? asOfDate = null);
    Task<NineColumnReportDto?> GetNineColumnReportByControlAccountAsync(Guid controlAccountId, DateTime? asOfDate = null, bool includeChildren = false);
    Task<List<NineColumnReportDto>> GetNineColumnReportTrendAsync(Guid projectId, DateTime startDate, DateTime endDate, string periodType = "monthly");
    Task<Core.DTOs.EVM.NineColumnReportValidationResult?> ValidateNineColumnReportDataAsync(Guid projectId, DateTime? asOfDate = null);
}

// Query parameters
public class EVMQueryParameters : QueryParameters
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public bool IncludeForecasts { get; set; } = true;
}