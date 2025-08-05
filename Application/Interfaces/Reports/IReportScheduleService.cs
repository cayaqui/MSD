using Core.DTOs.Common;
using Core.DTOs.Reports;

namespace Application.Interfaces.Reports;

public interface IReportScheduleService : IBaseService<ReportScheduleDto, CreateReportScheduleDto, UpdateReportScheduleDto>
{
    Task<IEnumerable<ReportScheduleDto>> GetActiveSchedulesAsync();
    Task<IEnumerable<ReportScheduleDto>> GetSchedulesByProjectAsync(Guid projectId);
    Task<IEnumerable<ReportScheduleDto>> GetSchedulesByTemplateAsync(Guid templateId);
    Task<ReportScheduleDto?> ActivateScheduleAsync(Guid scheduleId);
    Task<ReportScheduleDto?> DeactivateScheduleAsync(Guid scheduleId);
    Task<ReportDto?> RunScheduleNowAsync(Guid scheduleId);
    Task<IEnumerable<ReportScheduleExecutionDto>> GetScheduleExecutionsAsync(Guid scheduleId);
    Task<IEnumerable<ReportScheduleDto>> GetDueSchedulesAsync();
    Task<bool> ExecuteDueSchedulesAsync();
}
