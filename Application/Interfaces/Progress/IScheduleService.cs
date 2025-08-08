using Core.DTOs.Common;
using Core.DTOs.Progress.Schedules;
using Result = Core.Results.Result;

namespace Application.Interfaces.Progress;

public interface IScheduleService
{
    // Schedule Management
    Task<PagedResult<ScheduleVersionDto>> GetScheduleVersionsAsync(ScheduleFilterDto filter, CancellationToken cancellationToken = default);
    Task<ScheduleVersionDto?> GetScheduleVersionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ScheduleVersionDto?> GetActiveScheduleAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<List<ScheduleVersionDto>> GetProjectSchedulesAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Core.Results.Result<Guid>> CreateScheduleVersionAsync(CreateScheduleVersionDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> UpdateScheduleVersionAsync(Guid id, UpdateScheduleVersionDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeleteScheduleVersionAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    
    // Baseline Management
    Task<Result> SetAsBaselineAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> ApproveScheduleAsync(Guid scheduleId, Guid userId, string? comments = null, CancellationToken cancellationToken = default);
    Task<Result> RejectScheduleAsync(Guid scheduleId, Guid userId, string comments, CancellationToken cancellationToken = default);
    Task<Result> ArchiveScheduleAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken = default);
    
    // Schedule Analysis
    Task<Core.Results.Result<ScheduleHealthDto>> GetScheduleHealthAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task<Core.Results.Result<CriticalPathAnalysisDto>> GetCriticalPathAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task<Core.Results.Result<ScheduleComparisonDto>> CompareSchedulesAsync(Guid baselineId, Guid currentId, CancellationToken cancellationToken = default);
    Task<Core.Results.Result<List<ScheduleVarianceDto>>> GetScheduleVariancesAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    
    // Validation
    Task<Result> ValidateScheduleIntegrityAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task<Result> RecalculateScheduleAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    
    // Permissions
    Task<bool> CanModifyScheduleAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanApproveScheduleAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken = default);
    
    // Additional methods from module
    Task<ScheduleVersionDto?> GetCurrentScheduleAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<ScheduleVersionDto?> GetBaselineScheduleAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Result> SubmitForApprovalAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result> ApproveScheduleAsync(Guid scheduleId, ApproveScheduleDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<List<ScheduleVarianceDto>> GetProjectScheduleVariancesAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Core.Results.Result<Guid>> ImportScheduleAsync(ImportScheduleDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<byte[]> ExportScheduleAsync(Guid scheduleId, string format, CancellationToken cancellationToken = default);
    Task<byte[]> ExportScheduleTemplateAsync(string format, CancellationToken cancellationToken = default);
    Task<bool> CanCreateNewVersionAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<bool> CanSetAsBaselineAsync(Guid scheduleId, CancellationToken cancellationToken = default);
}