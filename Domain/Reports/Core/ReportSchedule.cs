namespace Domain.Reports.Core;

public class ReportSchedule : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Template and Project
    public Guid TemplateId { get; set; }
    public virtual ReportTemplate Template { get; set; } = null!;
    public Guid? ProjectId { get; set; }
    public virtual Project? Project { get; set; }
    
    // Schedule Configuration
    public ScheduleFrequency Frequency { get; set; }
    public string CronExpression { get; set; } = "0 0 * * *"; // Default to daily at midnight
    public DateTime? NextRunDate { get; set; }
    public DateTime? LastRunDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string TimeZoneId { get; set; } = "UTC";
    
    // Report Configuration
    public ExportFormat DefaultFormat { get; set; }
    public string ParametersJson { get; set; } = "{}"; // JSON object
    public string OptionsJson { get; set; } = "{}"; // JSON object
    
    // Distribution
    public string RecipientsJson { get; set; } = "[]"; // JSON array of email addresses
    public bool SendByEmail { get; set; }
    public string? EmailSubjectTemplate { get; set; }
    public string? EmailBodyTemplate { get; set; }
    public bool SaveToRepository { get; set; }
    public string? RepositoryPath { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public int ExecutionCount { get; set; }
    public int FailureCount { get; set; }
    public string? LastError { get; set; }
    
    // User who created the schedule
    public Guid CreatedByUserId { get; set; }
    public virtual User CreatedByUser { get; set; } = null!;
    
    // Navigation properties
    public virtual ICollection<ReportScheduleExecution> Executions { get; set; } = new List<ReportScheduleExecution>();
    
    public void UpdateNextRunDate()
    {
        // Simple implementation for now - can be enhanced with cron library later
        if (LastRunDate.HasValue)
        {
            NextRunDate = Frequency switch
            {
                ScheduleFrequency.Daily => LastRunDate.Value.AddDays(1),
                ScheduleFrequency.Weekly => LastRunDate.Value.AddDays(7),
                ScheduleFrequency.Monthly => LastRunDate.Value.AddMonths(1),
                ScheduleFrequency.Quarterly => LastRunDate.Value.AddMonths(3),
                ScheduleFrequency.Yearly => LastRunDate.Value.AddYears(1),
                _ => LastRunDate.Value.AddDays(1)
            };
        }
        else
        {
            NextRunDate = StartDate;
        }
        
        // Ensure NextRunDate is within the active period
        if (EndDate.HasValue && NextRunDate > EndDate.Value)
            NextRunDate = null;
    }
}
