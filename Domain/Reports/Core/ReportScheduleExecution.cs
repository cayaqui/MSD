using Core.Enums.Reports;
using Domain.Common;

namespace Domain.Reports.Core;

public class ReportScheduleExecution : BaseAuditableEntity
{
    public Guid ScheduleId { get; set; }
    public virtual ReportSchedule Schedule { get; set; } = null!;
    
    public DateTime ExecutionDate { get; set; }
    public ReportStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    
    public Guid? GeneratedReportId { get; set; }
    public virtual Report? GeneratedReport { get; set; }
    
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationSeconds { get; set; }
    
    public int? RecipientCount { get; set; }
    public int? SuccessfulDeliveries { get; set; }
    public int? FailedDeliveries { get; set; }
}