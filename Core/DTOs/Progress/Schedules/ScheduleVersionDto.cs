using Core.DTOs.Common;
using Core.Enums.Progress;

namespace Core.DTOs.Progress.Schedules;

public class ScheduleVersionDto : BaseDto
{
    public Guid ProjectId { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Status
    public ScheduleStatus Status { get; set; }
    public string StatusDisplay => Status switch
    {
        ScheduleStatus.Draft => "Borrador",
        ScheduleStatus.UnderReview => "En Revisión",
        ScheduleStatus.Approved => "Aprobado",
        ScheduleStatus.Baselined => "Línea Base",
        ScheduleStatus.Superseded => "Reemplazado",
        _ => Status.ToString()
    };
    public bool IsBaseline { get; set; }
    public DateTime? BaselineDate { get; set; }
    
    // Schedule Dates
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public int TotalDuration { get; set; }
    
    // Statistics
    public int TotalActivities { get; set; }
    public int CriticalActivities { get; set; }
    public decimal TotalFloat { get; set; }
    public DateTime? DataDate { get; set; }
    
    // Approval
    public DateTime? SubmittedDate { get; set; }
    public string? SubmittedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovedBy { get; set; }
    public string? ApprovalComments { get; set; }
    
    // Import Information
    public string? SourceSystem { get; set; }
    public DateTime? ImportDate { get; set; }
    public string? ImportedBy { get; set; }
    
    // Related Info
    public string ProjectName { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
    public int MilestoneCount { get; set; }
}

public class CreateScheduleVersionDto
{
    public Guid ProjectId { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? DataDate { get; set; }
    public string? SourceSystem { get; set; }
}

public class UpdateScheduleVersionDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? DataDate { get; set; }
}

public class ImportScheduleDto
{
    public Guid ProjectId { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty; // MSProject, Primavera, etc.
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
}

public class ScheduleComparisonDto
{
    public Guid BaselineScheduleId { get; set; }
    public Guid CurrentScheduleId { get; set; }
    public DateTime BaselineStartDate { get; set; }
    public DateTime CurrentStartDate { get; set; }
    public DateTime BaselineEndDate { get; set; }
    public DateTime CurrentEndDate { get; set; }
    public int StartVarianceDays { get; set; }
    public int EndVarianceDays { get; set; }
    public int BaselineDuration { get; set; }
    public int CurrentDuration { get; set; }
    public int DurationVarianceDays { get; set; }
    public int AddedActivities { get; set; }
    public int RemovedActivities { get; set; }
    public int ModifiedActivities { get; set; }
    public decimal OverallVariancePercentage { get; set; }
}

public class ScheduleVarianceDto
{
    public Guid ActivityId { get; set; }
    public string ActivityCode { get; set; } = string.Empty;
    public string ActivityName { get; set; } = string.Empty;
    public DateTime PlannedStart { get; set; }
    public DateTime? ActualStart { get; set; }
    public int StartVariance { get; set; }
    public DateTime PlannedEnd { get; set; }
    public DateTime? ActualEnd { get; set; }
    public int EndVariance { get; set; }
    public string VarianceType { get; set; } = string.Empty;
    public string ImpactLevel { get; set; } = string.Empty;
}

public class ApproveScheduleDto
{
    public string? Comments { get; set; }
    public bool SetAsBaseline { get; set; }
}

public class ScheduleFilterDto : BaseFilterDto
{
    public Guid? ProjectId { get; set; }
    public ScheduleStatus? Status { get; set; }
    public bool? IsBaseline { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
}

public class ScheduleHealthDto
{
    public Guid ScheduleId { get; set; }
    public int TotalActivities { get; set; }
    public int CompletedActivities { get; set; }
    public int InProgressActivities { get; set; }
    public int DelayedActivities { get; set; }
    public int OnTrackActivities { get; set; }
    public decimal OverallProgress { get; set; }
    public decimal SchedulePerformanceIndex { get; set; }
    public decimal CriticalPathHealth { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class CriticalPathAnalysisDto
{
    public Guid ScheduleId { get; set; }
    public int CriticalPathLength { get; set; }
    public List<CriticalActivityDto> CriticalActivities { get; set; } = new();
    public decimal TotalFloat { get; set; }
    public DateTime ProjectCompletionDate { get; set; }
    public DateTime ForecastCompletionDate { get; set; }
    public string CriticalPathStatus { get; set; } = string.Empty;
}

public class CriticalActivityDto
{
    public Guid Id { get; set; }
    public string ActivityCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public int DurationDays { get; set; }
    public decimal ProgressPercentage { get; set; }
    public string Status { get; set; } = string.Empty;
}