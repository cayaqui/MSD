using Core.DTOs.Common;
using Core.DTOs.Progress.Schedules;

namespace Web.Services.Interfaces.Progress;

public interface IScheduleApiService
{
    // Schedule Version Management
    Task<PagedResult<ScheduleVersionDto>> GetScheduleVersionsAsync(ScheduleFilterDto filter, CancellationToken cancellationToken = default);
    Task<ScheduleVersionDto?> GetScheduleVersionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ScheduleVersionDto?> GetCurrentScheduleAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<ScheduleVersionDto?> GetBaselineScheduleAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Guid> CreateScheduleVersionAsync(CreateScheduleVersionDto dto, CancellationToken cancellationToken = default);
    Task UpdateScheduleVersionAsync(Guid id, UpdateScheduleVersionDto dto, CancellationToken cancellationToken = default);
    Task DeleteScheduleVersionAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Schedule Operations
    Task SubmitForApprovalAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task ApproveScheduleAsync(Guid scheduleId, ApproveScheduleDto dto, CancellationToken cancellationToken = default);
    Task SetAsBaselineAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    
    // Import/Export
    Task<Guid> ImportScheduleAsync(Guid projectId, string version, string name, string sourceSystem, Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    Task<byte[]> ExportScheduleAsync(Guid scheduleId, string format = "MSProject", CancellationToken cancellationToken = default);
    Task<byte[]> DownloadScheduleTemplateAsync(string format = "MSProject", CancellationToken cancellationToken = default);
    
    // Comparison
    Task<ScheduleComparisonDto> CompareSchedulesAsync(Guid baselineId, Guid currentId, CancellationToken cancellationToken = default);
    Task<List<ScheduleVarianceDto>> GetScheduleVariancesAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> CanCreateNewVersionAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<bool> CanSetAsBaselineAsync(Guid scheduleId, CancellationToken cancellationToken = default);
}