using Core.Enums.Documents;
using Core.Enums.Projects;
using Core.Enums.Reports;

namespace Core.DTOs.Reports;

public class ReportScheduleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Template and Project
    public Guid TemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    
    // Schedule Configuration
    public ScheduleFrequency Frequency { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public DateTime? NextRunDate { get; set; }
    public DateTime? LastRunDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
    
    // Report Configuration
    public ExportFormat DefaultFormat { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public ReportOptionsDto Options { get; set; } = new();
    
    // Distribution
    public List<string> Recipients { get; set; } = new();
    public bool SendByEmail { get; set; }
    public string? EmailSubjectTemplate { get; set; }
    public string? EmailBodyTemplate { get; set; }
    public bool SaveToRepository { get; set; }
    public string? RepositoryPath { get; set; }
    
    // Status
    public bool IsActive { get; set; }
    public int ExecutionCount { get; set; }
    public int FailureCount { get; set; }
    public string? LastError { get; set; }
    
    // Audit
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}


public class CreateReportScheduleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TemplateId { get; set; }
    public Guid? ProjectId { get; set; }
    public ScheduleFrequency Frequency { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public ExportFormat DefaultFormat { get; set; } = ExportFormat.PDF;
    public List<string> Recipients { get; set; } = new();
    public bool SendByEmail { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsValid(out Dictionary<string, string[]> errors)
    {
        errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add(nameof(Name), new[] { "Name is required" });

        if (TemplateId == Guid.Empty)
            errors.Add(nameof(TemplateId), new[] { "Template ID is required" });

        if (string.IsNullOrWhiteSpace(CronExpression) && Frequency == ScheduleFrequency.Custom)
            errors.Add(nameof(CronExpression), new[] { "Cron expression is required for custom frequency" });

        if (SendByEmail && (!Recipients.Any() || Recipients.All(string.IsNullOrWhiteSpace)))
            errors.Add(nameof(Recipients), new[] { "Recipients are required when sending by email" });

        return errors.Count == 0;
    }
}

public class UpdateReportScheduleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ScheduleFrequency Frequency { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public ExportFormat DefaultFormat { get; set; }
    public List<string> Recipients { get; set; } = new();
    public bool SendByEmail { get; set; }
    public DateTime? EndDate { get; set; }

    public bool IsValid(out Dictionary<string, string[]> errors)
    {
        errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add(nameof(Name), new[] { "Name is required" });

        if (string.IsNullOrWhiteSpace(CronExpression) && Frequency == ScheduleFrequency.Custom)
            errors.Add(nameof(CronExpression), new[] { "Cron expression is required for custom frequency" });

        if (SendByEmail && (!Recipients.Any() || Recipients.All(string.IsNullOrWhiteSpace)))
            errors.Add(nameof(Recipients), new[] { "Recipients are required when sending by email" });

        return errors.Count == 0;
    }
}

//public enum ScheduleFrequency
//{
//    Once,
//    Daily,
//    Weekly,
//    BiWeekly,
//    Monthly,
//    Quarterly,
//    Yearly,
//    Custom
//}