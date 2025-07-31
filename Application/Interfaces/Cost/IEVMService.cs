
using Core.DTOs.EVM;

namespace Application.Interfaces.Cost;

/// <summary>
/// Interface for EVM (Earned Value Management) service
/// </summary>
public interface IEVMService
{
    // Query Operations
    Task<PagedResult<EVMRecordDto>> GetEVMRecordsAsync(
        Guid controlAccountId,
        QueryParameters parameters,
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
}
